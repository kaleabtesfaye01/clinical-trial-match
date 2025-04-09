using ClinicalTrialMatcher.Models;

namespace ClinicalTrialMatcher.Interfaces
{
    public interface ITrialMatchingService
    {
        // Match trials based on patient input and return top N matches.
        Task<ClinicalTrial[]> MatchTrialsAsync(PatientInput input, int topN = 10);
    }
}
