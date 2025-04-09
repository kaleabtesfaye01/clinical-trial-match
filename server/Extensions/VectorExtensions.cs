using System;

namespace ClinicalTrialMatcher.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns the cosine similarity between two float arrays.
        /// Cosine similarity measures the cosine of the angle between two vectors,
        /// which is useful for comparing direction regardless of magnitude.
        /// </summary>
        public static float CosineSimilarity(this float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
            {
                throw new ArgumentException("Vectors must have the same length.");
            }

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                throw new InvalidOperationException("One or both vectors are zero vectors.");
            }

            return dotProduct / (float)(Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }
    }
}
