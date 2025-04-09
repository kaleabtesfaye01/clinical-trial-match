using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;

namespace ClinicalTrialMatcher.Data
{
    public static class ListComparers
    {
        public static readonly ValueComparer<List<string>> StringListComparer =
            new(
                (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
                c => (c ?? new List<string>()).Aggregate(0, (current, next) => HashCode.Combine(current, next.GetHashCode())),
                c => (c ?? new List<string>()).ToList()
            );
    }
}
