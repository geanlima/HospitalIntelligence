using Hospital.AI.Application.Abstractions;
using Hospital.AI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Hospital.AI.Infrastructure.VectorStore;

public sealed class PgVectorStore : IVectorStore
{
    private readonly AiDbContext _dbContext;

    public PgVectorStore(
        AiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpsertAsync(
        KnowledgeChunk chunk,
        CancellationToken cancellationToken = default)
    {
        if (chunk.Embedding is null || chunk.Embedding.Length == 0)
        {
            throw new InvalidOperationException(
                "Embedding is required before upserting into pgvector.");
        }

        var existing =
            await _dbContext.KnowledgeDocuments
                .FirstOrDefaultAsync(
                    x => x.SourceId == chunk.SourceId,
                    cancellationToken);

        var vector = new Vector(chunk.Embedding);

        if (existing is null)
        {
            _dbContext.KnowledgeDocuments.Add(
                new KnowledgeDocument
                {
                    Id = Guid.NewGuid(),
                    SourceId = chunk.SourceId,
                    Title = chunk.Title,
                    Content = chunk.Content,
                    PatientId = chunk.PatientId,
                    Embedding = vector,
                    UpdatedAtUtc = DateTimeOffset.UtcNow
                });
        }
        else
        {
            existing.Title = chunk.Title;
            existing.Content = chunk.Content;
            existing.PatientId = chunk.PatientId;
            existing.Embedding = vector;
            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        EmbeddingVector query,
        int topK,
        Guid? patientId = null,
        CancellationToken cancellationToken = default)
    {
        var queryVector = new Vector(query.Values);

        var dbQuery = _dbContext.KnowledgeDocuments
            .AsNoTracking()
            .AsQueryable();

        if (patientId.HasValue)
        {
            dbQuery = dbQuery.Where(
                x => x.PatientId == null ||
                     x.PatientId == patientId.Value);
        }

        var rows = await dbQuery
            .OrderBy(x => x.Embedding.CosineDistance(queryVector))
            .Take(topK)
            .Select(x => new
            {
                x.SourceId,
                x.Title,
                x.Content,
                x.PatientId,
                Distance = x.Embedding.CosineDistance(queryVector)
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new RetrievedChunk(
                new KnowledgeChunk(
                    x.SourceId,
                    x.Title,
                    x.Content,
                    x.PatientId),
                Score: Math.Max(0, 1.0 - x.Distance)))
            .ToList()
            .AsReadOnly();
    }
}
