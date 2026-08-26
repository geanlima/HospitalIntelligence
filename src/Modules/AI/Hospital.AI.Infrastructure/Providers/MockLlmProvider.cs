using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Providers;

public sealed class MockLlmProvider : ILlmProvider
{
    public Task<LlmCompletionResult> CompleteAsync(
        LlmCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var userMessage =
            request.Messages.LastOrDefault(x => x.Role == "user")?.Content
            ?? string.Empty;

        var answer =
            "Resposta simulada (Mock LLM) com base no contexto recuperado.\n\n" +
            "Esta implementação existe para estudo: o Application chama ILlmProvider, " +
            "e no futuro você pode trocar este Mock por OpenAI/Azure sem alterar o Domain clínico.\n\n" +
            "Trecho do prompt recebido:\n" +
            Truncate(userMessage, 400);

        return Task.FromResult(
            new LlmCompletionResult(
                answer,
                "Mock",
                "mock-clinical-v1"));
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }
}
