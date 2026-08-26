using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.Index;

public sealed record IndexPatientClinicalRecordsCommand(
    Guid PatientId);

public sealed record IndexPatientClinicalRecordsResult(
    Guid PatientId,
    int IndexedCount,
    DateTimeOffset IndexedAtUtc);

public sealed class IndexPatientClinicalRecordsHandler
{
    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IClinicalRecordSource _clinicalRecordSource;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;

    public IndexPatientClinicalRecordsHandler(
        IAiAccessPolicy accessPolicy,
        IClinicalRecordSource clinicalRecordSource,
        IEmbeddingService embeddingService,
        IVectorStore vectorStore)
    {
        _accessPolicy = accessPolicy;
        _clinicalRecordSource = clinicalRecordSource;
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }

    public async Task<Result<IndexPatientClinicalRecordsResult>> HandleAsync(
        IndexPatientClinicalRecordsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.PatientId == Guid.Empty)
        {
            return Result<IndexPatientClinicalRecordsResult>.Failure(
                new Error(
                    "AI.Index.PatientIdRequired",
                    "PatientId é obrigatório para indexar o prontuário."));
        }

        var access =
            await _accessPolicy.EnsureCanAccessPatientAsync(
                command.PatientId,
                cancellationToken);

        if (access.IsFailure)
        {
            return Result<IndexPatientClinicalRecordsResult>.Failure(
                access.Error);
        }

        var records =
            await _clinicalRecordSource.GetByPatientIdAsync(
                command.PatientId,
                cancellationToken);

        var indexedAt = DateTimeOffset.UtcNow;
        var count = 0;

        foreach (var record in records)
        {
            var textToEmbed =
                $"{record.Title}\n{record.Content}";

            var embedding =
                await _embeddingService.EmbedAsync(
                    textToEmbed,
                    cancellationToken);

            await _vectorStore.UpsertAsync(
                new KnowledgeChunk(
                    record.SourceId,
                    record.Title,
                    record.Content,
                    record.PatientId,
                    embedding.Values),
                cancellationToken);

            count++;
        }

        return Result<IndexPatientClinicalRecordsResult>.Success(
            new IndexPatientClinicalRecordsResult(
                command.PatientId,
                count,
                indexedAt));
    }
}
