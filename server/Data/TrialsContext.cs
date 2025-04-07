using Microsoft.EntityFrameworkCore;
using _.Models;

namespace _.Data
{
    public class TrialsContext(DbContextOptions<TrialsContext> options) : DbContext(options)
    {
        public DbSet<ClinicalTrial> ClinicalTrials { get; set; }
        public DbSet<Intervention> Interventions { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure ClinicalTrial's primary key
            modelBuilder.Entity<ClinicalTrial>()
                .HasKey(ct => ct.NctId);

            // Configure one-to-many relationship between ClinicalTrial and Interventions
            modelBuilder.Entity<ClinicalTrial>()
                .HasMany(ct => ct.Interventions)
                .WithOne()  // If you don’t have a back-navigation property in Intervention
                .HasForeignKey("ClinicalTrialNctId");

            // Configure one-to-many relationship between ClinicalTrial and Locations
            modelBuilder.Entity<ClinicalTrial>()
                .HasMany(ct => ct.Locations)
                .WithOne()
                .HasForeignKey("ClinicalTrialNctId");

            // If you use collections of primitive types (Conditions, Keywords), consider adding value converters.
        }
    }
}
