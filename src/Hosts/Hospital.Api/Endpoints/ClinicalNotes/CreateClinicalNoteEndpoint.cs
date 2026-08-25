using Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;
using Hospital.ClinicalNotes.Domain.ClinicalNotes;

namespace Hospital.Api.Endpoints.ClinicalNotes;

public static class CreateClinicalNoteEndpoint
{
    public static IEndpointRouteBuilder MapCreateClinicalNoteEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/clinical-notes",
            async (
                CreateClinicalNoteRequest request,
                CreateClinicalNoteHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateClinicalNoteCommand(
                    request.PatientId,
                    request.Professional,
                    request.NoteType,
                    request.Content,
                    request.CreatedAtUtc);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(
                        new
                        {
                            result.Error.Code,
                            result.Error.Description
                        });
                }

                return Results.Created(
                    $"/clinical-notes/{result.Value.Value}",
                    new
                    {
                        Id = result.Value.Value
                    });
            })
            .WithName("CreateClinicalNote")
            .WithTags("Clinical Notes")
            .WithSummary("Registra uma evolução ou anotação clínica")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record CreateClinicalNoteRequest(
    Guid PatientId,
    string Professional,
    ClinicalNoteType NoteType,
    string Content,
    DateTimeOffset CreatedAtUtc);