using Hospital.AI.Infrastructure.Embeddings;
using Hospital.AI.Infrastructure.Seed;
using Hospital.AI.Infrastructure.VectorStore;

namespace Hospital.AI.UnitTests;

public class VectorStoreTests
{
    [Fact]
    public async Task SearchAsync_Should_Return_Ranked_Chunks()
    {
        var embedding = new DeterministicEmbeddingService();
        var store = new InMemoryVectorStore();

        await MockKnowledgeSeeder.SeedAsync(store, embedding);

        var query =
            await embedding.EmbedAsync(
                "saturação oxigênio alerta crítico");

        var results =
            await store.SearchAsync(query, topK: 2);

        Assert.Equal(2, results.Count);
        Assert.True(results[0].Score >= results[1].Score);
    }
}
