using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Application.Rag;

public sealed class RagRetriever : IRagRetriever
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStore _vectorStore;

    public RagRetriever(
        IEmbeddingService embeddingService,
        IVectorStore vectorStore)
    {
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }

    public async Task<RagContext> RetrieveAsync(
        string question,
        Guid? patientId,
        int topK = 3,
        CancellationToken cancellationToken = default)
    {
        var embedding =
            await _embeddingService.EmbedAsync(
                question,
                cancellationToken);

        var chunks =
            await _vectorStore.SearchAsync(
                embedding,
                topK,
                patientId,
                cancellationToken);

        return new RagContext(chunks);
    }
}
