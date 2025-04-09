using Microsoft.EntityFrameworkCore;
using ClinicalTrialMatcher.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector.EntityFrameworkCore; // Make sure this is installed and imported.

namespace ClinicalTrialMatcher.Data
{
    public class TrialsContext : DbContext
    {
        public TrialsContext(DbContextOptions<TrialsContext> options) : base(options)
        {
        }

        public DbSet<ClinicalTrial> ClinicalTrials { get; set; } = null!;
        public DbSet<Intervention> Interventions { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Converter for List<string>? to JSON for PostgreSQL.
            var listConverter = new ValueConverter<List<string>?, string>(
                v => v == null ? string.Empty : JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => string.IsNullOrEmpty(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>());

            // Apply conversion and a value comparer on Conditions.
            modelBuilder.Entity<ClinicalTrial>()
                .Property(e => e.Conditions)
                .HasConversion(listConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ListComparers.StringListComparer);

            // Apply conversion and a value comparer on Keywords.
            modelBuilder.Entity<ClinicalTrial>()
                .Property(e => e.Keywords)
                .HasConversion(listConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ListComparers.StringListComparer);

            // Apply conversion and a value comparer on Phases.
            modelBuilder.Entity<ClinicalTrial>()
                .Property(e => e.Phases)
                .HasConversion(listConverter)
                .HasColumnType("jsonb")
                .Metadata.SetValueComparer(ListComparers.StringListComparer);

            // Configure the vectorized data column.
            modelBuilder.Entity<ClinicalTrial>()
                .Property(e => e.VectorizedData)
                .HasColumnType("vector(1536)");

            // Configure cascade delete for dependent entities:
            modelBuilder.Entity<ClinicalTrial>()
                .HasMany(ct => ct.Locations)
                .WithOne(loc => loc.ClinicalTrial)
                .HasForeignKey(loc => loc.ClinicalTrialNctId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClinicalTrial>()
                .HasMany(ct => ct.Interventions)
                .WithOne(i => i.ClinicalTrial)
                .HasForeignKey(i => i.ClinicalTrialNctId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
