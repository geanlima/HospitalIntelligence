using Microsoft.EntityFrameworkCore;

namespace Hospital.AI.Infrastructure.Persistence;

public sealed class AiDbContext : DbContext
{
    public AiDbContext(
        DbContextOptions<AiDbContext> options)
        : base(options)
    {
    }

    public DbSet<KnowledgeDocument> KnowledgeDocuments =>
        Set<KnowledgeDocument>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.ApplyConfiguration(
            new KnowledgeDocumentConfiguration());
    }
}
