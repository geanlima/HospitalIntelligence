using Hospital.AI.Application.Abstractions;
using Hospital.AI.Infrastructure;
using Hospital.AI.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hospital.AI.UnitTests;

public sealed class LlmProviderRegistrationTests
{
    [Fact]
    public void AddAiInfrastructure_WithMockProvider_RegistersMockLlm()
    {
        using var provider = BuildServiceProvider("Mock");

        var llm = provider.GetRequiredService<ILlmProvider>();

        Assert.IsType<MockLlmProvider>(llm);
    }

    [Fact]
    public void AddAiInfrastructure_WithOpenAiCompatibleProvider_RegistersHttpClientLlm()
    {
        using var provider = BuildServiceProvider("OpenAICompatible");

        var llm = provider.GetRequiredService<ILlmProvider>();

        Assert.IsType<OpenAiCompatibleLlmProvider>(llm);
    }

    [Fact]
    public void AddAiInfrastructure_WithOllamaAlias_RegistersHttpClientLlm()
    {
        using var provider = BuildServiceProvider("Ollama");

        var llm = provider.GetRequiredService<ILlmProvider>();

        Assert.IsType<OpenAiCompatibleLlmProvider>(llm);
    }

    private static ServiceProvider BuildServiceProvider(string providerName)
    {
        var json = $$"""
            {
              "ConnectionStrings": {
                "AiDatabase": "Host=localhost;Database=hospital;Username=hospital;Password=hospital"
              },
              "AI": {
                "Provider": "{{providerName}}",
                "VectorStore": "InMemory",
                "OpenAICompatible": {
                  "BaseUrl": "http://localhost:11434/v1/",
                  "Model": "llama3.2"
                }
              }
            }
            """;

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAiInfrastructure(configuration);
        return services.BuildServiceProvider();
    }
}
