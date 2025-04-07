using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using _.Services;

namespace _.Services
{
    public class ClinicalTrialsBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ClinicalTrialsBackgroundService> logger) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<ClinicalTrialsBackgroundService> _logger = logger;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(24); // Refresh every 24 hours



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting clinical trials data refresh at: {time}", DateTimeOffset.Now);
                try
                {
                    // Create a new scope for every refresh cycle
                    using var scope = _scopeFactory.CreateScope();
                    // Resolve ClinicalTrialsService from the scoped provider
                    var trialsService = scope.ServiceProvider.GetRequiredService<ClinicalTrialsService>();
                    var trials = await trialsService.FetchAllTrialsAsync();
                    _logger.LogInformation("Data refresh complete: {count} records loaded.", trials.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing clinical trials data.");
                }
                await Task.Delay(_refreshInterval, stoppingToken);
            }
        }
    }
}
