using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClinicalTrialMatcher.Models
{
    public class Intervention
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Auto‑generated primary key

        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }

        // Foreign key reference to the parent ClinicalTrial
        [ForeignKey("ClinicalTrialNctId")]
        public string ClinicalTrialNctId { get; set; } = null!;
        [JsonIgnore]
        public ClinicalTrial ClinicalTrial { get; set; } = null!;
    }
}
