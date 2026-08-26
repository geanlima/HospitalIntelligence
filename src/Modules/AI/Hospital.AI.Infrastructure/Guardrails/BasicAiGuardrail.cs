using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Guardrails;

public sealed class BasicAiGuardrail : IAiGuardrail
{
    private static readonly string[] BlockedInputPhrases =
    [
        "ignore previous instructions",
        "ignore all instructions",
        "jailbreak"
    ];

    public GuardrailResult ValidateInput(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new GuardrailResult(
                false,
                "Entrada vazia.");
        }

        if (text.Length > 4000)
        {
            return new GuardrailResult(
                false,
                "Pergunta excede o tamanho máximo permitido.");
        }

        var normalized = text.ToLowerInvariant();

        if (BlockedInputPhrases.Any(p => normalized.Contains(p)))
        {
            return new GuardrailResult(
                false,
                "Padrão de prompt injection detectado.");
        }

        return new GuardrailResult(true);
    }

    public GuardrailResult ValidateOutput(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new GuardrailResult(
                false,
                "Modelo retornou resposta vazia.");
        }

        return new GuardrailResult(true);
    }
}
