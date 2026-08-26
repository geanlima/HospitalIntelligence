namespace Hospital.AI.Infrastructure;

public sealed class AiOptions
{
    public const string SectionName = "AI";

    /// <summary>
    /// Mock | OpenAICompatible | OpenAI | Ollama
    /// OpenAICompatible (e aliases) usam Chat Completions HTTP
    /// (OpenAI, Azure OpenAI ou Ollama local).
    /// </summary>
    public string Provider { get; set; } = "Mock";

    /// <summary>
    /// PgVector (padrão) ou InMemory (útil para testes isolados).
    /// </summary>
    public string VectorStore { get; set; } = "PgVector";

    public int DefaultTopK { get; set; } = 3;

    public OpenAiCompatibleOptions OpenAICompatible { get; set; } = new();
}

public sealed class OpenAiCompatibleOptions
{
    /// <summary>
    /// Exemplos:
    /// - OpenAI: https://api.openai.com/v1
    /// - Ollama: http://localhost:11434/v1
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434/v1";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "llama3.2";

    public int TimeoutSeconds { get; set; } = 60;
}
