using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using _.Models;
using _.Data;
using Microsoft.EntityFrameworkCore;

namespace _.Services
{
    public class ClinicalTrialsService(HttpClient httpClient, TrialsContext dbContext)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly TrialsContext _dbContext = dbContext;

        /// <summary>
        /// Fetches all pages of clinical trial data using the nextPageToken for pagination.
        /// The first request includes countTotal and pageSize parameters.
        /// Subsequent requests use the same fields and filter plus pageToken.
        /// </summary>
        public async Task<List<ClinicalTrial>> FetchAllTrialsAsync()
        {
            // Base query string: include all fields you selected.
            string fieldsAndFilter = "fields=protocolSection.identificationModule.nctId," +
                                       "protocolSection.identificationModule.briefTitle," +
                                       "protocolSection.statusModule.overallStatus," +
                                       "protocolSection.descriptionModule.briefSummary," +
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
                                       "protocolSection.eligibilityModule.genderBased," +
                                       "protocolSection.eligibilityModule.genderDescription," +
                                       "protocolSection.eligibilityModule.minimumAge," +
                                       "protocolSection.eligibilityModule.maximumAge," +
                                       "protocolSection.statusModule.startDateStruct.date," +
                                       "protocolSection.statusModule.studyFirstSubmitDate," +
                                       "protocolSection.statusModule.studyFirstPostDateStruct.date," +
                                       "protocolSection.statusModule.lastUpdateSubmitDate," +
                                       "protocolSection.statusModule.lastUpdatePostDateStruct.date," +
                                       "protocolSection.designModule.phases";

            string filter = "&filter.overallStatus=ENROLLING_BY_INVITATION,NOT_YET_RECRUITING,RECRUITING";

            // For the first page, include countTotal and pageSize
            string firstPageParams = $"?{fieldsAndFilter}{filter}&countTotal=true&pageSize=1000";
            // Subsequent pages: same fields and filter (omit countTotal and pageSize)
            string subsequentParams = $"?{fieldsAndFilter}{filter}&pageSize=1000";

            string baseUrl = "https://clinicaltrials.gov/api/v2/studies";
            var allTrials = new List<ClinicalTrial>();
            string url = baseUrl + firstPageParams;
            string? pageToken = null;

            do
            {
                if (!string.IsNullOrEmpty(pageToken))
                {
                    url = baseUrl + subsequentParams + $"&pageToken={Uri.EscapeDataString(pageToken)}";
                }

                // Log for debugging
                Console.WriteLine($"Fetching URL: {url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(jsonString);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("studies", out JsonElement studiesElement))
                {
                    studiesElement = root.GetProperty("StudyFieldsResponse").GetProperty("StudyFields");
                }

                var trialsPage = ParseTrials(studiesElement);
                Console.WriteLine($"Parsed {trialsPage.Count} trials from current page.");
                allTrials.AddRange(trialsPage);

                // Get the next page token
                pageToken = root.TryGetProperty("nextPageToken", out JsonElement tokenElement)
                    ? tokenElement.GetString()
                    : null;

            } while (!string.IsNullOrEmpty(pageToken));

            foreach (var trial in allTrials)
            {
                // Try to find an existing trial by primary key
                var existingTrial = await _dbContext.ClinicalTrials.FindAsync(trial.NctId);
                if (existingTrial == null)
                {
                    _dbContext.ClinicalTrials.Add(trial);
                }
                else
                {
                    // Update properties as needed, for example:
                    existingTrial.BriefTitle = trial.BriefTitle;
                    existingTrial.OverallStatus = trial.OverallStatus;
                    existingTrial.BriefSummary = trial.BriefSummary;
                    // ...update all other properties
                    // For related entities, you may need to decide how to update them
                }
            }
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Upserted all trials to the database.");


            return allTrials;
        }

        // Helper method to map JSON array to ClinicalTrial objects.
        private static List<ClinicalTrial> ParseTrials(JsonElement studiesElement)
        {
            List<ClinicalTrial> trials = [];

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

                ClinicalTrial trial = new()
                {
                    NctId = GetSafeString(identification, "nctId"),
                    BriefTitle = GetSafeString(identification, "briefTitle"),
                    OverallStatus = GetSafeString(statusModule, "overallStatus"),
                    BriefSummary = GetSafeString(descriptionModule, "briefSummary"),
                    Conditions = GetSafeStringArray(conditionsModule, "conditions"),
                    Keywords = GetSafeStringArray(conditionsModule, "keywords"),
                    StudyType = GetSafeString(designModule, "studyType"),
                    EligibilityCriteria = GetSafeString(eligibilityModule, "eligibilityCriteria"),
                    HealthyVolunteers = GetSafeString(eligibilityModule, "healthyVolunteers"),
                    Sex = GetSafeString(eligibilityModule, "sex"),
                    GenderBased = GetSafeString(eligibilityModule, "genderBased"),
                    GenderDescription = GetSafeString(eligibilityModule, "genderDescription"),
                    MinimumAge = GetSafeString(eligibilityModule, "minimumAge"),
                    MaximumAge = GetSafeString(eligibilityModule, "maximumAge"),
                    StartDate = GetSafeNestedString(statusModule, "startDateStruct", "date"),
                    StudyFirstSubmitDate = GetSafeString(statusModule, "studyFirstSubmitDate"),
                    StudyFirstPostDate = GetSafeNestedString(statusModule, "studyFirstPostDateStruct", "date"),
                    LastUpdateSubmitDate = GetSafeString(statusModule, "lastUpdateSubmitDate"),
                    LastUpdatePostDate = GetSafeNestedString(statusModule, "lastUpdatePostDateStruct", "date"),
                    Phases = GetSafeArrayFirstValue(designModule, "phases")
                };

                // Map interventions
                if (armsInterventionsModule.ValueKind == JsonValueKind.Object &&
                    armsInterventionsModule.TryGetProperty("interventions", out JsonElement interventionsElement) &&
                    interventionsElement.ValueKind == JsonValueKind.Array)
                {
                    trial.Interventions = [.. interventionsElement.EnumerateArray().Select(intervention => new Intervention
                    {
                        Name = GetSafeString(intervention, "name"),
                        Type = GetSafeString(intervention, "type"),
                        Description = GetSafeString(intervention, "description")
                    })];
                }
                else
                {
                    trial.Interventions = [];
                }

                // Map locations
                if (contactsLocationsModule.ValueKind == JsonValueKind.Object &&
                    contactsLocationsModule.TryGetProperty("locations", out JsonElement locationsElement) &&
                    locationsElement.ValueKind == JsonValueKind.Array)
                {
                    trial.Locations = [.. locationsElement.EnumerateArray().Select(location => new Location
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
                    trial.Locations = [];
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
            List<string> list = [];
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out JsonElement arrayElement) &&
                arrayElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arrayElement.EnumerateArray())
                {
                    list.Add(item.ValueKind == JsonValueKind.String ? item.GetString() ?? "" : item.ToString());
                }
            }
            return list;
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

        private static string GetSafeArrayFirstValue(JsonElement element, string propertyName)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out JsonElement arrayElement) &&
                arrayElement.ValueKind == JsonValueKind.Array)
            {
                return arrayElement.EnumerateArray().Select(e => e.GetString()).FirstOrDefault() ?? "";
            }
            return "";
        }
    }
}
