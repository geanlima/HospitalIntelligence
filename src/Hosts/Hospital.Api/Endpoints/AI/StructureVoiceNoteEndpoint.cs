using Hospital.AI.Application.ClinicalSafety;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class StructureVoiceNoteEndpoint
{
    public static IEndpointRouteBuilder MapStructureVoiceNoteEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/clinical-safety/voice-note",
                async (
                    StructureVoiceNoteRequest request,
                    StructureVoiceNoteHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new StructureVoiceNoteCommand(
                                request.Transcript,
                                request.PatientId,
                                request.NoteType),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new StructureVoiceNoteResponse(
                            value.DraftTitle,
                            value.StructuredContent,
                            value.NoteType,
                            value.Provider,
                            value.PatientId));
                })
            .WithTags("AI")
            .WithName("StructureVoiceNote")
            .WithSummary("Estrutura rascunho de nota a partir de transcrição")
            .WithDescription(
                "Prontuário por voz sem hardware: cola a transcrição (STT externo) e recebe rascunho estruturado.")
            .Produces<StructureVoiceNoteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
