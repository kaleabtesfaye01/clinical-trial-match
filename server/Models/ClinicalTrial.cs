using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Pgvector; // Using the Pgvector type from the Pgvector package

namespace ClinicalTrialMatcher.Models
{
    public class ClinicalTrial
    {
        [Key]
        public required string NctId { get; set; }
        public string? BriefTitle { get; set; }
        public string? OverallStatus { get; set; }
        public string? StartDate { get; set; }
        public string? StudyFirstSubmitDate { get; set; }
        public string? StudyFirstPostDate { get; set; }
        public string? LastUpdateSubmitDate { get; set; }
        public string? LastUpdatePostDate { get; set; }
        public string? BriefSummary { get; set; }
        public List<string>? Conditions { get; set; }
        public List<string>? Keywords { get; set; }
        public string? StudyType { get; set; }
        public List<string>? Phases { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Intervention>? Interventions { get; set; }
        public string? EligibilityCriteria { get; set; }
        public string? HealthyVolunteers { get; set; }
        public string? Sex { get; set; }
        public string? MinimumAge { get; set; }
        public string? MaximumAge { get; set; }

        [Column("min_age_months")]
        public int? MinAgeInMonths { get; set; }

        [Column("max_age_months")]
        public int? MaxAgeInMonths { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Location>? Locations { get; set; }

        // The column for vectorized embedding (1536 dimensions; adjust as needed)
        [Column(TypeName = "vector(1536)")]
        public Vector? VectorizedData { get; set; }
    }
}
