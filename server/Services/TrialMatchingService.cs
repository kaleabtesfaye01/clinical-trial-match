using ClinicalTrialMatcher.Data;
using ClinicalTrialMatcher.Interfaces;
using ClinicalTrialMatcher.Models;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialMatcher.Extensions;

namespace ClinicalTrialMatcher.Services
{
    public class TrialMatchingService(TrialsContext dbContext, IVectorizationService vectorService) : ITrialMatchingService
    {
        private readonly TrialsContext _dbContext = dbContext;
        private readonly IVectorizationService _vectorService = vectorService;

        /// <summary>
        /// Matches clinical trials with patient input.
        /// Applies database‑side filters (for sex and location) if the corresponding patient inputs are provided,
        /// then loads the results into memory to further filter by condition and age.
        /// Finally, computes the vector (Euclidean) distance between the trial’s embedding and the patient’s notes embedding,
        /// and returns the top N matches.
        /// </summary>
        public async Task<ClinicalTrial[]> MatchTrialsAsync(PatientInput input, int topN = 10)
        {
            // 1. Generate the embedding vector for the patient’s notes and conditions combined.
            //    (Note: The vectorization service should handle the actual embedding generation.)
            var patientEmbedding = await _vectorService.GenerateEmbeddingAsync("Patient Notes: " + input.Notes ?? "" + "Conditions/Symptoms: " + input.Condition ?? "");
            if (patientEmbedding == null || patientEmbedding.Length == 0)
            {
                throw new Exception("Failed to generate embedding for the patient input.");
            }

            // 2. Build the initial query for trials that are already vectorized.
            IQueryable<ClinicalTrial> query = _dbContext.ClinicalTrials
                .Include(ct => ct.Interventions)
                .Include(ct => ct.Locations)
                .AsSplitQuery()
                .Where(ct => ct.VectorizedData != null);

            // 3. Apply a sex filter if input.Sex is provided.
            if (!string.IsNullOrWhiteSpace(input.Sex))
            {
                string sexInput = input.Sex.Trim();
                query = query.Where(ct => ct.Sex != null &&
                    (EF.Functions.ILike(ct.Sex, $"%{sexInput}%") || EF.Functions.ILike(ct.Sex, "%ALL%")));
            }

            // 4. Apply location filters if input.City, input.State, or input.Country are provided
            if (!string.IsNullOrWhiteSpace(input.City))
            {
                string cityInput = input.City.Trim();
                query = query.Where(ct => ct.Locations != null && ct.Locations.Any(loc =>
                    EF.Functions.ILike(loc.City ?? "", $"%{cityInput}%")));
            }

            if (!string.IsNullOrWhiteSpace(input.State))
            {
                string stateInput = input.State.Trim();
                query = query.Where(ct => ct.Locations != null &&
                    ct.Locations.Any(loc => EF.Functions.ILike(loc.State ?? "", $"%{stateInput}%")));
            }

            if (!string.IsNullOrWhiteSpace(input.Country))
            {
                string countryInput = input.Country.Trim();
                query = query.Where(ct => ct.Locations != null &&
                    ct.Locations.Any(loc => EF.Functions.ILike(loc.Country ?? "", $"%{countryInput}%")));
            }

            // 5. Apply an age filter if input.Age is provided.
            if (input.Age.HasValue)
            {
                int patientAgeInMonths = input.Age.Value * 12;
                query = query.Where(ct =>
                    ct.MinAgeInMonths <= patientAgeInMonths &&
                    (ct.MaxAgeInMonths == 0 || ct.MaxAgeInMonths >= patientAgeInMonths));
            }

            // 6. Execute the query.
            //    (Note: This will only fetch trials that are already vectorized.)
            var filteredTrials = await query.ToListAsync();
            if (filteredTrials == null || filteredTrials.Count == 0)
            {
                throw new Exception("No trials found matching the provided filters.");
            }

            // 7. Find matches by calculating the squared Euclidean distance between the patient’s embedding and each trial’s embedding.
            var matched_trials = filteredTrials
                .Select(trial => new
                {
                    Trial = trial,
                    Similarity = trial.VectorizedData?.ToArray()?.CosineSimilarity(patientEmbedding) ?? float.MinValue
                })
                .OrderByDescending(m => m.Similarity)
                .Take(topN)
                .Select(m => m.Trial)
                .ToArray();


            if (matched_trials == null || matched_trials.Length == 0)
            {
                throw new Exception("No matches found for the provided patient input.");
            }

            // 8. Return the top N matches.
            return matched_trials;

        }
    }
}
