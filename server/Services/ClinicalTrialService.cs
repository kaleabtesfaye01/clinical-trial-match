using ClinicalTrialMatcher.Data;
using ClinicalTrialMatcher.Interfaces;
using ClinicalTrialMatcher.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Pgvector;
using ClinicalTrialMatcher.Utilities; // For the Vector type

namespace ClinicalTrialMatcher.Services
{
    public class ClinicalTrialsService : IClinicalTrialsService
    {
        private readonly HttpClient _httpClient;
        private readonly TrialsContext _dbContext;
        private readonly IVectorizationService _vectorService;

        public ClinicalTrialsService(HttpClient httpClient, TrialsContext dbContext, IVectorizationService vectorService)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _vectorService = vectorService;
        }

        // This method fetches 1,000 trials from the external API, vectorizes them,
        // and saves them to the database—unless the database already has 1,000 (or more) entries.
        public async Task<List<ClinicalTrial>> FetchAndVectorizeTrialsAsync()
        {
            // Check if there are already at least 1000 trials in the database.
            int existingCount = await _dbContext.ClinicalTrials.CountAsync();
            if (existingCount >= 1000)
            {
                Console.WriteLine("Database already contains 1000 or more trials. Skipping fetch and vectorization.");
                // Retrieve and return the existing trials (you can adjust the ordering if needed).
                return await _dbContext.ClinicalTrials
                    .Include(ct => ct.Interventions)
                    .Include(ct => ct.Locations)
                    .OrderBy(ct => ct.NctId)
                    .AsSplitQuery()
                    .ToListAsync();
            }

            // Define query parameters for fetching 1,000 trials.
            string fieldsAndFilter = "fields=protocolSection.identificationModule.nctId," +
                                       "protocolSection.identificationModule.briefTitle," +
                                       "protocolSection.statusModule.overallStatus," +
                                       "protocolSection.descriptionModule.briefSummary," +
                                       "protocolSection.identificationModule.officialTitle," +
                                       "protocolSection.conditionsModule.conditions," +
                                       "protocolSection.conditionsModule.keywords," +
                                       "protocolSection.designModule.studyType," +
                                       "protocolSection.armsInterventionsModule.interventions.name," +
                                       "protocolSection.armsInterventionsModule.interventions.type," +
                                       "protocolSection.armsInterventionsModule.interventions.description," +
                                       "protocolSection.contactsLocationsModule.locations.status," +
                                       "protocolSection.contactsLocationsModule.locations.city," +
                                       "protocolSection.contactsLocationsModule.locations.state," +
                                       "protocolSection.contactsLocationsModule.locations.zip," +
                                       "protocolSection.contactsLocationsModule.locations.country," +
                                       "protocolSection.eligibilityModule.eligibilityCriteria," +
                                       "protocolSection.eligibilityModule.healthyVolunteers," +
                                       "protocolSection.eligibilityModule.sex," +
                                       "protocolSection.eligibilityModule.minimumAge," +
                                       "protocolSection.eligibilityModule.maximumAge," +
                                       "protocolSection.statusModule.startDateStruct.date," +
                                       "protocolSection.statusModule.studyFirstSubmitDate," +
                                       "protocolSection.statusModule.studyFirstPostDateStruct.date," +
                                       "protocolSection.statusModule.lastUpdateSubmitDate," +
                                       "protocolSection.statusModule.lastUpdatePostDateStruct.date," +
                                       "protocolSection.designModule.phases";
            string filter = "&filter.overallStatus=RECRUITING";
            string parameters = $"?{fieldsAndFilter}{filter}&pageSize=1000";

            string baseUrl = "https://clinicaltrials.gov/api/v2/studies";
            string url = baseUrl + parameters;
            Console.WriteLine($"Fetching URL: {url}");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            // Get the study fields array (the API response might nest the array).
            JsonElement studiesElement;
            if (!root.TryGetProperty("studies", out studiesElement))
            {
                studiesElement = root.GetProperty("StudyFieldsResponse").GetProperty("StudyFields");
            }

            var trials = ParseTrials(studiesElement);
            Console.WriteLine($"Parsed {trials.Count} trials.");

            // Process each trial: combine text from various fields and vectorize.
            foreach (var trial in trials)
            {
                string combinedText = string.Join(" ",
                    trial.BriefSummary ?? "",
                    trial.EligibilityCriteria ?? "",
                    trial.Conditions != null ? string.Join(", ", trial.Conditions) : "",
                    trial.Keywords != null ? string.Join(", ", trial.Keywords) : "");

                try
                {
                    // Generate an embedding from the combined text.
                    var embedding = await _vectorService.GenerateEmbeddingAsync(combinedText);
                    // Convert the float array to a Pgvector.Vector instance.
                    trial.VectorizedData = new Vector(embedding);
                    Console.WriteLine($"Trial {trial.NctId} vectorized.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error vectorizing trial {trial.NctId}: {ex.Message}");
                }
            }

            // Clear the existing table and save new trials.
            _dbContext.ClinicalTrials.RemoveRange(_dbContext.ClinicalTrials);
            await _dbContext.SaveChangesAsync();
            _dbContext.ClinicalTrials.AddRange(trials);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Total trials saved in DB: {await _dbContext.ClinicalTrials.CountAsync()}");

            return trials;
        }

        // Helper method to parse JSON into ClinicalTrial objects.
        private static List<ClinicalTrial> ParseTrials(JsonElement studiesElement)
        {
            var trials = new List<ClinicalTrial>();

            foreach (var study in studiesElement.EnumerateArray())
            {
                var protocolSection = study.GetProperty("protocolSection");

                protocolSection.TryGetProperty("identificationModule", out JsonElement identification);
                protocolSection.TryGetProperty("statusModule", out JsonElement statusModule);
                protocolSection.TryGetProperty("descriptionModule", out JsonElement descriptionModule);
                protocolSection.TryGetProperty("conditionsModule", out JsonElement conditionsModule);
                protocolSection.TryGetProperty("designModule", out JsonElement designModule);
                protocolSection.TryGetProperty("eligibilityModule", out JsonElement eligibilityModule);
                protocolSection.TryGetProperty("armsInterventionsModule", out JsonElement armsInterventionsModule);
                protocolSection.TryGetProperty("contactsLocationsModule", out JsonElement contactsLocationsModule);

                var trial = new ClinicalTrial
                {
                    NctId = GetSafeString(identification, "nctId"),
                    BriefTitle = GetSafeString(identification, "briefTitle"),
                    OfficialTitle = GetSafeString(identification, "officialTitle"),
                    OverallStatus = GetSafeString(statusModule, "overallStatus"),
                    StartDate = GetSafeNestedString(statusModule, "startDateStruct", "date"),
                    StudyFirstSubmitDate = GetSafeString(statusModule, "studyFirstSubmitDate"),
                    StudyFirstPostDate = GetSafeNestedString(statusModule, "studyFirstPostDateStruct", "date"),
                    LastUpdateSubmitDate = GetSafeString(statusModule, "lastUpdateSubmitDate"),
                    LastUpdatePostDate = GetSafeNestedString(statusModule, "lastUpdatePostDateStruct", "date"),
                    BriefSummary = GetSafeString(descriptionModule, "briefSummary"),
                    Conditions = GetSafeStringArray(conditionsModule, "conditions"),
                    Keywords = GetSafeStringArray(conditionsModule, "keywords"),
                    StudyType = GetSafeString(designModule, "studyType"),
                    Phases = GetSafeStringArray(designModule, "phases"),
                    EligibilityCriteria = GetSafeString(eligibilityModule, "eligibilityCriteria"),
                    HealthyVolunteers = GetSafeString(eligibilityModule, "healthyVolunteers"),
                    Sex = GetSafeString(eligibilityModule, "sex"),
                    MinimumAge = GetSafeString(eligibilityModule, "minimumAge"),
                    MaximumAge = GetSafeString(eligibilityModule, "maximumAge"),
                    MinAgeInMonths = AgeHelper.NormalizeAgeToMonths(GetSafeString(eligibilityModule, "minimumAge")),
                    MaxAgeInMonths = AgeHelper.NormalizeAgeToMonths(GetSafeString(eligibilityModule, "maximumAge"))
                };

                // Map interventions.
                if (armsInterventionsModule.ValueKind == JsonValueKind.Object &&
                    armsInterventionsModule.TryGetProperty("interventions", out JsonElement interventionsElement) &&
                    interventionsElement.ValueKind == JsonValueKind.Array)
                {
                    trial.Interventions = [.. interventionsElement.EnumerateArray()
                        .Select(intervention => new Intervention
                        {
                            Name = GetSafeString(intervention, "name"),
                            Type = GetSafeString(intervention, "type"),
                            Description = GetSafeString(intervention, "description")
                        })];
                }
                else
                {
                    trial.Interventions = new List<Intervention>();
                }

                // Map locations.
                if (contactsLocationsModule.ValueKind == JsonValueKind.Object &&
                    contactsLocationsModule.TryGetProperty("locations", out JsonElement locationsElement) &&
                    locationsElement.ValueKind == JsonValueKind.Array)
                {
                    trial.Locations = [.. locationsElement.EnumerateArray()
                        .Select(location => new Location
                        {
                            Status = GetSafeString(location, "status"),
                            City = GetSafeString(location, "city"),
                            State = GetSafeString(location, "state"),
                            Zip = GetSafeString(location, "zip"),
                            Country = GetSafeString(location, "country")
                        })];
                }
                else
                {
                    trial.Locations = new List<Location>();
                }

                trials.Add(trial);
            }

            return trials;
        }

        private static string GetSafeString(JsonElement element, string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out JsonElement prop))
            {
                return prop.ValueKind == JsonValueKind.String ? prop.GetString() ?? "" : prop.ToString();
            }
            return "";
        }

        private static List<string> GetSafeStringArray(JsonElement element, string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out JsonElement arrayElement) &&
                arrayElement.ValueKind == JsonValueKind.Array)
            {
                return [.. arrayElement.EnumerateArray().Select(e => e.GetString() ?? "")];
            }
            return new List<string>();
        }

        private static string GetSafeNestedString(JsonElement parent, string nestedProperty, string propertyName)
        {
            if (parent.ValueKind == JsonValueKind.Object &&
                parent.TryGetProperty(nestedProperty, out JsonElement nested) &&
                nested.ValueKind == JsonValueKind.Object)
            {
                return GetSafeString(nested, propertyName);
            }
            return "";
        }
    }
}
