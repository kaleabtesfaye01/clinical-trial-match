using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ClinicalTrialMatcher.Interfaces;

namespace ClinicalTrialMatcher.Services
{
    public class VectorizationService : IVectorizationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public VectorizationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new Exception("OpenAI API key not configured.");
        }

        public async Task<float[]> GenerateEmbeddingAsync(string input)
        {
            var requestBody = new
            {
                model = "text-embedding-3-small",
                input,
            };

            var jsonPayload = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings")
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            if (doc.RootElement.TryGetProperty("data", out JsonElement dataArray) &&
                dataArray.ValueKind == JsonValueKind.Array &&
                dataArray.EnumerateArray().FirstOrDefault().TryGetProperty("embedding", out JsonElement embeddingElement))
            {
                return [.. embeddingElement.EnumerateArray().Select(e => e.GetSingle())];
            }

            throw new Exception("Unable to parse the embedding from the API response.");
        }
    }
}
