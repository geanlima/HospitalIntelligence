namespace Hospital.AI.Application.Abstractions;

public sealed record EmbeddingVector(
    float[] Values);

public interface IEmbeddingService
{
    Task<EmbeddingVector> EmbedAsync(
        string text,
        CancellationToken cancellationToken = default);
}
