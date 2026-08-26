namespace Hospital.AI.Application.Abstractions;

public sealed record LlmMessage(
    string Role,
    string Content);

public sealed record LlmCompletionRequest(
    IReadOnlyList<LlmMessage> Messages,
    float Temperature = 0.2f);

public sealed record LlmCompletionResult(
    string Content,
    string ProviderName,
    string ModelName);

public interface ILlmProvider
{
    Task<LlmCompletionResult> CompleteAsync(
        LlmCompletionRequest request,
        CancellationToken cancellationToken = default);
}
