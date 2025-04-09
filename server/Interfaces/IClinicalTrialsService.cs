using ClinicalTrialMatcher.Models;

namespace ClinicalTrialMatcher.Interfaces
{
    public interface IClinicalTrialsService
    {
        // Fetch 1,000 trials, vectorize them, and save them to the database.
        Task<List<ClinicalTrial>> FetchAndVectorizeTrialsAsync();
    }
}
