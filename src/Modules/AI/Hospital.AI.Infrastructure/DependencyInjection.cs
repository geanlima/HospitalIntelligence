using Hospital.AI.Application.Abstractions;
using Hospital.AI.Application.Ask;
using Hospital.AI.Application.Audit;
using Hospital.AI.Application.ClinicalSafety;
using Hospital.AI.Application.Index;
using Hospital.AI.Application.Rag;
using Hospital.AI.Application.Search;
using Hospital.AI.Infrastructure.Access;
using Hospital.AI.Infrastructure.Audit;
using Hospital.AI.Infrastructure.Embeddings;
using Hospital.AI.Infrastructure.Guardrails;
using Hospital.AI.Infrastructure.Persistence;
using Hospital.AI.Infrastructure.Prompts;
using Hospital.AI.Infrastructure.Providers;
using Hospital.AI.Infrastructure.Seed;
using Hospital.AI.Infrastructure.VectorStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pgvector.EntityFrameworkCore;

namespace Hospital.AI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAiInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AiOptions>(
            configuration.GetSection(AiOptions.SectionName));

        var connectionString =
            configuration.GetConnectionString("AiDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AiDatabase' was not found.");
        }

        services.AddDbContext<AiDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString,
                    o => o.UseVector()));

        services.AddSingleton<IEmbeddingService, DeterministicEmbeddingService>();
        services.AddSingleton<IPromptCatalog, InMemoryPromptCatalog>();
        services.AddSingleton<IAiGuardrail, BasicAiGuardrail>();
        services.AddSingleton<IAiAuditStore, InMemoryAiAuditStore>();
        services.AddScoped<IAiAccessPolicy, PatientScopedAiAccessPolicy>();
        services.AddSingleton<MockLlmProvider>();

        services.AddHttpClient<OpenAiCompatibleLlmProvider>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;
            var settings = options.OpenAICompatible;

            client.BaseAddress = new Uri(
                EnsureTrailingSlash(settings.BaseUrl));

            client.Timeout = TimeSpan.FromSeconds(
                Math.Max(5, settings.TimeoutSeconds));
        });

        services.AddScoped<ILlmProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;

            if (string.Equals(
                    options.Provider,
                    "OpenAICompatible",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    options.Provider,
                    "OpenAI",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    options.Provider,
                    "Ollama",
                    StringComparison.OrdinalIgnoreCase))
            {
                return sp.GetRequiredService<OpenAiCompatibleLlmProvider>();
            }

            return sp.GetRequiredService<MockLlmProvider>();
        });

        services.AddSingleton<InMemoryVectorStore>();

        services.AddScoped<IVectorStore>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;

            if (string.Equals(
                    options.VectorStore,
                    "InMemory",
                    StringComparison.OrdinalIgnoreCase))
            {
                return sp.GetRequiredService<InMemoryVectorStore>();
            }

            return sp.GetRequiredService<PgVectorStore>();
        });

        services.AddScoped<PgVectorStore>();
        services.AddScoped<IRagRetriever, RagRetriever>();
        services.AddScoped<AskAiHandler>();
        services.AddScoped<IndexPatientClinicalRecordsHandler>();
        services.AddScoped<SearchClinicalKnowledgeHandler>();
        services.AddScoped<AuditPatientChartHandler>();
        services.AddScoped<AssessClinicalSafetyHandler>();
        services.AddScoped<StructureVoiceNoteHandler>();

        return services;
    }

    public static async Task SeedAiKnowledgeAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var options =
            scope.ServiceProvider.GetRequiredService<IOptions<AiOptions>>().Value;

        if (!string.Equals(
                options.VectorStore,
                "InMemory",
                StringComparison.OrdinalIgnoreCase))
        {
            var dbContext =
                scope.ServiceProvider.GetRequiredService<AiDbContext>();

            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        var vectorStore =
            scope.ServiceProvider.GetRequiredService<IVectorStore>();

        var embeddingService =
            scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

        await MockKnowledgeSeeder.SeedAsync(
            vectorStore,
            embeddingService,
            cancellationToken);
    }

    private static string EnsureTrailingSlash(string baseUrl)
    {
        return baseUrl.EndsWith('/')
            ? baseUrl
            : baseUrl + "/";
    }
}
