using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;
using Hospital.Salux.Checkpoints;
using Hospital.Salux.Configuration;
using Hospital.Salux.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hospital.Salux.Adapters;

/// <summary>
/// Adapter ACL do Salux: lê pacientes reais e emite IntegrationMessage canônico.
/// O avanço de checkpoint fica a cargo do SaluxPatientSyncService após processar.
/// </summary>
public sealed class SaluxPatientAdapter : IExternalSystemAdapter
{
    public const string PatientsStreamKey = "SALUX.Patients";

    private readonly ISaluxPatientReader _reader;
    private readonly SaluxPatientMapper _mapper;
    private readonly ISaluxCheckpointStore _checkpoints;
    private readonly SaluxOptions _options;
    private readonly ILogger<SaluxPatientAdapter> _logger;

    public SaluxPatientAdapter(
        ISaluxPatientReader reader,
        SaluxPatientMapper mapper,
        ISaluxCheckpointStore checkpoints,
        IOptions<SaluxOptions> options,
        ILogger<SaluxPatientAdapter> logger)
    {
        _reader = reader;
        _mapper = mapper;
        _checkpoints = checkpoints;
        _options = options.Value;
        _logger = logger;
    }

    public string SourceSystem => "SALUX";

    public ISaluxCheckpointStore Checkpoints => _checkpoints;

    public async Task<SaluxPatientBatch> ReceiveBatchAsync(
        CancellationToken cancellationToken = default)
    {
        var checkpoint =
            await _checkpoints.GetAsync(
                PatientsStreamKey,
                cancellationToken);

        var batchSize = Math.Clamp(_options.BatchSize, 1, 2000);

        var patients =
            await _reader.ReadSinceAsync(
                checkpoint,
                batchSize,
                cancellationToken);

        if (patients.Count == 0)
        {
            return new SaluxPatientBatch([], checkpoint, checkpoint);
        }

        var messages = patients
            .Select(_mapper.Map)
            .ToList();

        var nextCheckpoint = patients.Max(p =>
            p.UpdatedAtUtc == default
                ? checkpoint
                : p.UpdatedAtUtc);

        _logger.LogInformation(
            "Salux patient batch fetched. Count={Count}, From={From:o}, To={To:o}",
            messages.Count,
            checkpoint,
            nextCheckpoint);

        return new SaluxPatientBatch(messages, checkpoint, nextCheckpoint);
    }

    public async Task<IntegrationMessage> ReceiveAsync(
        CancellationToken cancellationToken = default)
    {
        var batch = await ReceiveBatchAsync(cancellationToken);

        if (batch.Messages.Count == 0)
        {
            throw new InvalidOperationException(
                "No Salux patient messages available for the current checkpoint.");
        }

        return batch.Messages[0];
    }
}

public sealed record SaluxPatientBatch(
    IReadOnlyList<IntegrationMessage> Messages,
    DateTimeOffset CurrentCheckpointUtc,
    DateTimeOffset NextCheckpointUtc);
