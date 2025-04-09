namespace ClinicalTrialMatcher.Interfaces
{
    public interface IVectorizationService
    {
        // Generate an embedding (as an array of floats) for the given input text.
        Task<float[]> GenerateEmbeddingAsync(string input);
    }
}
