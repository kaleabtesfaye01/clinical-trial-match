using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _.Data
{
    public class TrialsContextFactory : IDesignTimeDbContextFactory<TrialsContext>
    {
        public TrialsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TrialsContext>();
            // Use a connection string suitable for design-time; adjust as needed.
            optionsBuilder.UseSqlite("Data Source=trials.db");

            return new TrialsContext(optionsBuilder.Options);
        }
    }
}
