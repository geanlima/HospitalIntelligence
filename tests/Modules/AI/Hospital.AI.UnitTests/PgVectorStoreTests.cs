using Hospital.AI.Infrastructure.Embeddings;
using Hospital.AI.Infrastructure.Persistence;
using Hospital.AI.Infrastructure.Seed;
using Hospital.AI.Infrastructure.VectorStore;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace Hospital.AI.UnitTests;

public class PgVectorStoreTests
{
    [Fact]
    public async Task SearchAsync_Should_Return_Ranked_Chunks_From_Postgres()
    {
        var connectionString =
            "Host=localhost;Port=5432;Database=hospital_intelligence;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AiDbContext>()
            .UseNpgsql(connectionString, o => o.UseVector())
            .Options;

        await using var dbContext = new AiDbContext(options);

        try
        {
            await dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
        }
        catch
        {
            // Ambiente sem PostgreSQL: pula o teste de integração.
            return;
        }

        await dbContext.Database.MigrateAsync();

        var embedding = new DeterministicEmbeddingService();
        var store = new PgVectorStore(dbContext);

        await MockKnowledgeSeeder.SeedAsync(store, embedding);

        var query =
            await embedding.EmbedAsync(
                "saturação oxigênio alerta crítico");

        var results =
            await store.SearchAsync(query, topK: 2);

        Assert.Equal(2, results.Count);
        Assert.True(results[0].Score >= results[1].Score);
        Assert.Contains(
            results,
            x => x.Chunk.SourceId.Contains("alert") ||
                 x.Chunk.Content.Contains("SpO2", StringComparison.OrdinalIgnoreCase) ||
                 x.Chunk.Content.Contains("satura", StringComparison.OrdinalIgnoreCase));
    }
}
