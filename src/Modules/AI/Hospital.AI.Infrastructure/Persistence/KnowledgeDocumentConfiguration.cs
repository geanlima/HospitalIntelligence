using Hospital.AI.Infrastructure.Embeddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.AI.Infrastructure.Persistence;

public sealed class KnowledgeDocumentConfiguration
    : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(
        EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable("ai_knowledge_documents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceId)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(x => x.SourceId)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.PatientId);

        builder.Property(x => x.Embedding)
            .HasColumnType($"vector({DeterministicEmbeddingService.Dimensions})")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.PatientId);
    }
}
