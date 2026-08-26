using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;

public sealed class SearchClinicalNotesHandler
{
    private readonly IClinicalNoteRepository _clinicalNoteRepository;

    public SearchClinicalNotesHandler(
        IClinicalNoteRepository clinicalNoteRepository)
    {
        _clinicalNoteRepository = clinicalNoteRepository;
    }

    public async Task<IReadOnlyCollection<ClinicalNoteResponse>> HandleAsync(
        SearchClinicalNotesQuery query,
        CancellationToken cancellationToken = default)
    {
        var notes =
            await _clinicalNoteRepository.SearchAsync(
                query.NoteType,
                cancellationToken);

        return notes
            .Select(x => new ClinicalNoteResponse(
                x.Id.Value,
                x.PatientId,
                x.Professional,
                x.NoteType.ToString(),
                x.Content,
                x.CreatedAtUtc))
            .ToList()
            .AsReadOnly();
    }
}
