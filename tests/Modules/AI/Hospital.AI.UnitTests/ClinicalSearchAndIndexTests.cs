using Hospital.AI.Application.Abstractions;
using Hospital.AI.Application.Index;
using Hospital.AI.Application.Rag;
using Hospital.AI.Application.Search;
using Hospital.AI.Infrastructure.Embeddings;
using Hospital.AI.Infrastructure.Guardrails;
using Hospital.AI.Infrastructure.VectorStore;

namespace Hospital.AI.UnitTests;

public class ClinicalSearchAndIndexTests
{
    [Fact]
    public async Task Index_Then_Search_Should_Return_Patient_Evidence()
    {
        var patientId = Guid.NewGuid();
        var source = new FakeClinicalRecordSource();
        source.AddRecord(
            new ClinicalRecordSnapshot(
                $"note:{Guid.NewGuid()}",
                "ClinicalNote",
                "Nota clínica - Evolucao",
                "Paciente com dispneia e SpO2 baixa em UTI.",
                patientId,
                DateTimeOffset.UtcNow));

        var embedding = new DeterministicEmbeddingService();
        var store = new InMemoryVectorStore();
        var access = new AllowAllAiAccessPolicy();

        var indexer = new IndexPatientClinicalRecordsHandler(
            access,
            source,
            embedding,
            store);

        var indexResult = await indexer.HandleAsync(
            new IndexPatientClinicalRecordsCommand(patientId));

        Assert.True(indexResult.IsSuccess);
        Assert.Equal(1, indexResult.Value.IndexedCount);

        var searcher = new SearchClinicalKnowledgeHandler(
            access,
            new BasicAiGuardrail(),
            new RagRetriever(embedding, store));

        var searchResult = await searcher.HandleAsync(
            new SearchClinicalKnowledgeQuery(
                "dispneia saturação",
                patientId,
                TopK: 3));

        Assert.True(searchResult.IsSuccess);
        Assert.NotEmpty(searchResult.Value.Hits);
        Assert.Contains(
            searchResult.Value.Hits,
            h => h.SourceId.StartsWith("note:", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Search_Should_Require_PatientId()
    {
        var searcher = new SearchClinicalKnowledgeHandler(
            new AllowAllAiAccessPolicy(),
            new BasicAiGuardrail(),
            new RagRetriever(
                new DeterministicEmbeddingService(),
                new InMemoryVectorStore()));

        var result = await searcher.HandleAsync(
            new SearchClinicalKnowledgeQuery(
                "qualquer",
                Guid.Empty));

        Assert.True(result.IsFailure);
        Assert.Equal("AI.Search.PatientIdRequired", result.Error.Code);
    }

    [Fact]
    public async Task Search_Should_Deny_When_Access_Policy_Fails()
    {
        var searcher = new SearchClinicalKnowledgeHandler(
            new DenyAiAccessPolicy(),
            new BasicAiGuardrail(),
            new RagRetriever(
                new DeterministicEmbeddingService(),
                new InMemoryVectorStore()));

        var result = await searcher.HandleAsync(
            new SearchClinicalKnowledgeQuery(
                "dispneia",
                Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Equal("AI.Access.PatientNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Index_Should_Fail_When_Access_Denied()
    {
        var indexer = new IndexPatientClinicalRecordsHandler(
            new DenyAiAccessPolicy(),
            new FakeClinicalRecordSource(),
            new DeterministicEmbeddingService(),
            new InMemoryVectorStore());

        var result = await indexer.HandleAsync(
            new IndexPatientClinicalRecordsCommand(Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Equal("AI.Access.PatientNotFound", result.Error.Code);
    }
}
