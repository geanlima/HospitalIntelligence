using Pgvector;

namespace Hospital.AI.Infrastructure.Persistence;

public sealed class KnowledgeDocument
{
    public Guid Id { get; set; }

    public string SourceId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid? PatientId { get; set; }

    public Vector Embedding { get; set; } = new(Array.Empty<float>());

    public DateTimeOffset UpdatedAtUtc { get; set; }
}
