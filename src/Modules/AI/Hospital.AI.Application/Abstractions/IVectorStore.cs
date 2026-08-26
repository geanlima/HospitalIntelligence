namespace Hospital.AI.Application.Abstractions;

public sealed record KnowledgeChunk(
    string SourceId,
    string Title,
    string Content,
    Guid? PatientId,
    float[]? Embedding = null);

public sealed record RetrievedChunk(
    KnowledgeChunk Chunk,
    double Score);

public interface IVectorStore
{
    Task UpsertAsync(
        KnowledgeChunk chunk,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        EmbeddingVector query,
        int topK,
        Guid? patientId = null,
        CancellationToken cancellationToken = default);
}
