using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hospital.AI.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Hospital.AI.Infrastructure.Providers;

/// <summary>
/// Cliente HTTP no formato OpenAI Chat Completions.
/// Serve para estudo com Ollama (local/grátis) ou OpenAI/Azure.
/// </summary>
public sealed class OpenAiCompatibleLlmProvider : ILlmProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly AiOptions _options;

    public OpenAiCompatibleLlmProvider(
        HttpClient httpClient,
        IOptions<AiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<LlmCompletionResult> CompleteAsync(
        LlmCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var settings = _options.OpenAICompatible;

        var payload = new ChatCompletionRequestDto(
            settings.Model,
            request.Messages
                .Select(m => new ChatMessageDto(m.Role, m.Content))
                .ToArray(),
            request.Temperature);

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "chat/completions")
        {
            Content = JsonContent.Create(payload, options: SerializerOptions)
        };

        if (!string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        }

        using var response =
            await _httpClient.SendAsync(httpRequest, cancellationToken);

        var raw =
            await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"LLM provider returned {(int)response.StatusCode}: {Truncate(raw, 500)}");
        }

        var parsed =
            JsonSerializer.Deserialize<ChatCompletionResponseDto>(
                raw,
                SerializerOptions);

        var content =
            parsed?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                "LLM provider returned an empty completion.");
        }

        return new LlmCompletionResult(
            content.Trim(),
            "OpenAICompatible",
            settings.Model);
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "...";
    }

    private sealed record ChatCompletionRequestDto(
        string Model,
        IReadOnlyList<ChatMessageDto> Messages,
        float Temperature);

    private sealed record ChatMessageDto(
        string Role,
        string Content);

    private sealed record ChatCompletionResponseDto(
        IReadOnlyList<ChatChoiceDto>? Choices);

    private sealed record ChatChoiceDto(
        ChatMessageDto? Message);
}
