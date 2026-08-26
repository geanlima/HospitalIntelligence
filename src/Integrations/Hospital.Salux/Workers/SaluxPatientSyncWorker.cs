using Hospital.Salux.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hospital.Salux.Workers;

public sealed class SaluxPatientSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<SaluxOptions> _options;
    private readonly ILogger<SaluxPatientSyncWorker> _logger;

    public SaluxPatientSyncWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<SaluxOptions> options,
        ILogger<SaluxPatientSyncWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = _options.Value;

        if (!options.Enabled)
        {
            _logger.LogInformation("Salux sync worker is disabled.");
            return;
        }

        var delay = TimeSpan.FromSeconds(
            Math.Clamp(options.PollIntervalSeconds, 15, 3600));

        _logger.LogInformation(
            "Salux sync worker started. Provider={Provider}, Interval={Interval}s",
            options.Provider,
            delay.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var sync =
                    scope.ServiceProvider.GetRequiredService<Sync.SaluxPatientSyncService>();

                await sync.SyncOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Salux patient sync cycle failed.");
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
