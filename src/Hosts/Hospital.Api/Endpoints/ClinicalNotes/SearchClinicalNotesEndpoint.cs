using Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;
using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.ClinicalNotes;

public static class SearchClinicalNotesEndpoint
{
    public static IEndpointRouteBuilder MapSearchClinicalNotesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/clinical-notes",
                async (
                    ClinicalNoteType? noteType,
                    SearchClinicalNotesHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchClinicalNotesQuery(
                        noteType);

                    var notes =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<ClinicalNoteListResponse>();

                    foreach (var note in notes)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(note.PatientId),
                                cancellationToken);

                        response.Add(
                            new ClinicalNoteListResponse(
                                note.Id,
                                note.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                note.Professional,
                                note.NoteType,
                                note.Content,
                                note.CreatedAtUtc));
                    }

                    return Results.Ok(response);
                })
            .WithTags("ClinicalNotes")
            .WithName("SearchClinicalNotes")
            .WithSummary("Lista notas clínicas")
            .WithDescription(
                "Lista notas clínicas com filtro opcional por tipo.");

        return app;
    }
}
