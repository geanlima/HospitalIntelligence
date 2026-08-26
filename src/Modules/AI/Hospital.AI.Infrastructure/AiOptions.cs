namespace Hospital.AI.Infrastructure;

public sealed class AiOptions
{
    public const string SectionName = "AI";

    /// <summary>
    /// Mock = LLM simulado. Outros providers virão depois.
    /// </summary>
    public string Provider { get; set; } = "Mock";

    /// <summary>
    /// PgVector (padrão) ou InMemory (útil para testes isolados).
    /// </summary>
    public string VectorStore { get; set; } = "PgVector";

    public int DefaultTopK { get; set; } = 3;
}
