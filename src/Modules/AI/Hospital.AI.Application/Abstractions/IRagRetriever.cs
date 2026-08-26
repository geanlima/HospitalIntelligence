namespace Hospital.AI.Application.Abstractions;

public sealed record RagContext(
    IReadOnlyList<RetrievedChunk> Chunks);

public interface IRagRetriever
{
    Task<RagContext> RetrieveAsync(
        string question,
        Guid? patientId,
        int topK = 3,
        CancellationToken cancellationToken = default);
}
