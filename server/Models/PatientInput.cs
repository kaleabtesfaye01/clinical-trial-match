namespace ClinicalTrialMatcher.Models
{
    public class PatientInput
    {
        /// <summary>
        /// Unstructured patient notes (required).
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Patient’s age in years (optional).
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Patient’s sex (e.g., "Male" or "Female") (optional).
        /// </summary>
        public string? Sex { get; set; }

        /// <summary>
        /// A condition or symptom to filter by (optional).
        /// </summary>
        public string? Condition { get; set; }

        /// <summary>
        /// Patient’s city (optional).
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Patient’s state (optional).
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Patient’s country (optional).
        /// </summary>
        public string? Country { get; set; }
    }
}
