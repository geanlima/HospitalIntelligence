using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Seed;

public static class MockKnowledgeSeeder
{
    public static async Task SeedAsync(
        IVectorStore vectorStore,
        IEmbeddingService embeddingService,
        CancellationToken cancellationToken = default)
    {
        var samples = new[]
        {
            new KnowledgeChunk(
                "note-demo-001",
                "Nota clínica - Evolução",
                "Paciente internado em UTI com queixa de dispneia. Equipe manteve monitorização contínua de saturação.",
                null),
            new KnowledgeChunk(
                "vitals-demo-001",
                "Sinais vitais recentes",
                "Última medição: SpO2 91%, FC 112 bpm, PA 95/60 mmHg. Equipe avaliou possível dessaturação.",
                null),
            new KnowledgeChunk(
                "alert-demo-001",
                "Alerta crítico",
                "Alerta de saturação baixa gerado automaticamente. Severidade Critical. Status Active.",
                null),
            new KnowledgeChunk(
                "exam-demo-001",
                "Exame - Gasometria",
                "Gasometria arterial solicitada em status InProgress para investigar hipoxemia.",
                null)
        };

        foreach (var sample in samples)
        {
            var embedding =
                await embeddingService.EmbedAsync(
                    sample.Content,
                    cancellationToken);

            await vectorStore.UpsertAsync(
                sample with { Embedding = embedding.Values },
                cancellationToken);
        }
    }
}
