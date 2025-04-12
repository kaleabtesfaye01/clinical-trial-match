using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using ClinicalTrialMatcher.Interfaces;

namespace ClinicalTrialMatcher.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseVectorization(this WebApplication app)
        {
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

            // Create a task completion source to handle async operations
            var initializationTask = new TaskCompletionSource();

            lifetime.ApplicationStarted.Register(() =>
            {
                // Run the async work on a background thread
                Task.Run(async () =>
                {
                    try
                    {
                        using var scope = app.Services.CreateScope();
                        var clinicalTrialsService = scope.ServiceProvider.GetRequiredService<IClinicalTrialsService>();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                        logger.LogInformation("Starting initial vectorization...");
                        await clinicalTrialsService.FetchAndVectorizeTrialsAsync();
                        logger.LogInformation("Initial vectorization completed successfully");

                        initializationTask.SetResult();
                    }
                    catch (Exception ex)
                    {
                        var logger = app.Services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred during initial vectorization");
                        initializationTask.SetException(ex);
                    }
                });
            });

            return app;
        }
    }
}