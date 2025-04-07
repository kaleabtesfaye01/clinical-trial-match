using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _.Models
{
    public class ClinicalTrial
    {
        [Key]
        public required string NctId { get; set; }
        public string? BriefTitle { get; set; }
        public string? OverallStatus { get; set; }
        public string? BriefSummary { get; set; }
        public List<string>? Conditions { get; set; }
        public List<string>? Keywords { get; set; }
        public string? StudyType { get; set; }
        public string? EligibilityCriteria { get; set; }
        public string? HealthyVolunteers { get; set; }
        public string? Sex { get; set; }
        public string? GenderBased { get; set; }
        public string? GenderDescription { get; set; }
        public string? MinimumAge { get; set; }
        public string? MaximumAge { get; set; }
        public string? StartDate { get; set; }
        public string? StudyFirstSubmitDate { get; set; }
        public string? StudyFirstPostDate { get; set; }
        public string? LastUpdateSubmitDate { get; set; }
        public string? LastUpdatePostDate { get; set; }
        public string? Phases { get; set; }

        // Navigation properties for related entities
        public List<Intervention> Interventions { get; set; } = [];
        public List<Location> Locations { get; set; } = [];
    }
}
