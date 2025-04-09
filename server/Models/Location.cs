using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClinicalTrialMatcher.Models
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Auto‑generated primary key

        public string? Status { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }

        // Foreign key reference to the parent ClinicalTrial
        [ForeignKey("ClinicalTrial")]
        public string ClinicalTrialNctId { get; set; } = null!;
        [JsonIgnore]
        public ClinicalTrial ClinicalTrial { get; set; } = null!;
    }
}
