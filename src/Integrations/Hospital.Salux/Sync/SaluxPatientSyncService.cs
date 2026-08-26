using Hospital.Integration.Idempotency;
using Hospital.Salux.Adapters;
using Hospital.Salux.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hospital.Salux.Sync;

public sealed class SaluxPatientSyncService
{
    private readonly SaluxPatientAdapter _adapter;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly ILogger<SaluxPatientSyncService> _logger;

    public SaluxPatientSyncService(
        SaluxPatientAdapter adapter,
        IServiceScopeFactory scopeFactory,
        IIdempotencyStore idempotencyStore,
        ILogger<SaluxPatientSyncService> logger)
    {
        _adapter = adapter;
        _scopeFactory = scopeFactory;
        _idempotencyStore = idempotencyStore;
        _logger = logger;
    }

    public async Task<SaluxSyncResult> SyncOnceAsync(
        CancellationToken cancellationToken = default)
    {
        var batch = await _adapter.ReceiveBatchAsync(cancellationToken);

        var processed = 0;
        var skipped = 0;
        var failed = 0;

        foreach (var message in batch.Messages)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var handler =
                    scope.ServiceProvider.GetRequiredService<SaluxPatientMessageHandler>();

                var idempotent = new IdempotentMessageHandler(
                    handler,
                    _idempotencyStore);

                var alreadyProcessed =
                    await _idempotencyStore.HasBeenProcessedAsync(
                        message.MessageId,
                        cancellationToken);

                if (alreadyProcessed)
                {
                    skipped++;
                    continue;
                }

                await idempotent.HandleAsync(message, cancellationToken);
                processed++;
            }
            catch (Exception ex)
            {
                failed++;
                _logger.LogError(
                    ex,
                    "Failed to process Salux message {MessageId}",
                    message.MessageId);
            }
        }

        // Só avança checkpoint se o lote inteiro foi consumido sem falha.
        if (failed == 0 && batch.Messages.Count > 0)
        {
            await _adapter.Checkpoints.SaveAsync(
                SaluxPatientAdapter.PatientsStreamKey,
                batch.NextCheckpointUtc,
                cancellationToken);
        }

        _logger.LogInformation(
            "Salux sync finished. Fetched={Fetched}, Processed={Processed}, Skipped={Skipped}, Failed={Failed}",
            batch.Messages.Count,
            processed,
            skipped,
            failed);

        return new SaluxSyncResult(
            batch.Messages.Count,
            processed,
            skipped,
            failed,
            batch.NextCheckpointUtc,
            DateTimeOffset.UtcNow);
    }
}

public sealed record SaluxSyncResult(
    int Fetched,
    int Processed,
    int Skipped,
    int Failed,
    DateTimeOffset CheckpointUtc,
    DateTimeOffset CompletedAtUtc);
