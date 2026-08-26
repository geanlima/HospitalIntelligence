using Hospital.AI.Application.Ask;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class AskAiEndpoint
{
    public static IEndpointRouteBuilder MapAskAiEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/ask",
                async (
                    AskAiRequest request,
                    AskAiHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var query = new AskAiQuery(
                        request.Question,
                        request.PatientId,
                        string.IsNullOrWhiteSpace(request.PromptKey)
                            ? "clinical-assistant"
                            : request.PromptKey);

                    var result =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    var response = new AskAiResponse(
                        value.Answer,
                        value.PromptKey,
                        value.Provider,
                        value.Sources
                            .Select(s => new AiSourceDto(
                                s.SourceId,
                                s.Title,
                                s.Excerpt,
                                s.Score))
                            .ToList(),
                        value.InteractionId,
                        value.OccurredAtUtc);

                    return Results.Ok(response);
                })
            .WithTags("AI")
            .WithName("AskAi")
            .WithSummary("Pergunta clínica assistida por IA (RAG)")
            .WithDescription(
                "Executa Guardrail → Prompt → RAG → LLM → Auditoria. " +
                "Para prontuário use PromptKey clinical-chart-qa + PatientId " +
                "(indexar antes via POST /ai/index/patients/{id}).")
            .Produces<AskAiResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
