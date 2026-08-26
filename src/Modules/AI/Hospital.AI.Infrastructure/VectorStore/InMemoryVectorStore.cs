using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.VectorStore;

public sealed class InMemoryVectorStore : IVectorStore
{
    private readonly List<KnowledgeChunk> _chunks = [];
    private readonly object _sync = new();

    public Task UpsertAsync(
        KnowledgeChunk chunk,
        CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            _chunks.RemoveAll(x => x.SourceId == chunk.SourceId);
            _chunks.Add(chunk);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        EmbeddingVector query,
        int topK,
        Guid? patientId = null,
        CancellationToken cancellationToken = default)
    {
        List<KnowledgeChunk> snapshot;

        lock (_sync)
        {
            snapshot = _chunks
                .Where(x =>
                    patientId is null ||
                    x.PatientId is null ||
                    x.PatientId == patientId)
                .ToList();
        }

        var ranked = snapshot
            .Select(chunk =>
            {
                var score = chunk.Embedding is null
                    ? KeywordScore(query, chunk.Content)
                    : CosineSimilarity(query.Values, chunk.Embedding);

                return new RetrievedChunk(chunk, score);
            })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<RetrievedChunk>>(ranked);
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        var length = Math.Min(a.Length, b.Length);
        double dot = 0;
        double magA = 0;
        double magB = 0;

        for (var i = 0; i < length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        if (magA == 0 || magB == 0)
        {
            return 0;
        }

        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }

    private static double KeywordScore(EmbeddingVector query, string content)
    {
        // Fallback educativo quando o chunk ainda não tem embedding materializado.
        return content.Length == 0 ? 0 : 0.1 + (query.Values.Average() * 0.01);
    }
}
