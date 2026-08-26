namespace Hospital.AI.Application.Abstractions;

public sealed record GuardrailResult(
    bool IsAllowed,
    string? Reason = null);

public interface IAiGuardrail
{
    GuardrailResult ValidateInput(string text);

    GuardrailResult ValidateOutput(string text);
}
