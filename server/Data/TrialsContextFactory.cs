using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClinicalTrialMatcher.Data
{
    public class TrialsContextFactory : IDesignTimeDbContextFactory<TrialsContext>
    {
        public TrialsContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TrialsContextFactory>()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string not found.");

            var optionsBuilder = new DbContextOptionsBuilder<TrialsContext>();
            optionsBuilder.UseNpgsql(connectionString, opts => opts.UseVector());

            return new TrialsContext(optionsBuilder.Options);
        }
    }
}
