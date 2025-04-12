using ClinicalTrialMatcher.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialMatcher.Extensions;
public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TrialsContext>();

        dbContext.Database.Migrate();
    }
}