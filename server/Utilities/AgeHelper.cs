namespace ClinicalTrialMatcher.Utilities
{
    public static class AgeHelper
    {
        public static int? NormalizeAgeToMonths(string ageString)
        {
            if (string.IsNullOrWhiteSpace(ageString))
                return null;

            var parts = ageString.ToLower().Split(' ');
            if (parts.Length != 2)
                return 0;

            if (!int.TryParse(parts[0], out int value))
                return 0;

            return parts[1] switch
            {
                "years" => value * 12,
                "year" => value * 12,
                "months" => value,
                "month" => value,
                "weeks" => value / 4, // approximate
                "week" => value / 4,  // approximate
                // return null for any other unit
                _ => null
            };
        }
    }
}