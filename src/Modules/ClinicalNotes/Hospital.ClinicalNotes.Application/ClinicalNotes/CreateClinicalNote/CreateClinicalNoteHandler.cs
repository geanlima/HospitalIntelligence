using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Hospital.SharedKernel.Application;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;

public sealed class CreateClinicalNoteHandler
{
    private readonly IClinicalNoteRepository _repository;

    public CreateClinicalNoteHandler(
        IClinicalNoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ClinicalNoteId>> HandleAsync(
        CreateClinicalNoteCommand command,
        CancellationToken cancellationToken = default)
    {
        var clinicalNote = ClinicalNote.Create(
            command.PatientId,
            command.Professional,
            command.NoteType,
            command.Content,
            command.CreatedAtUtc);

        await _repository.AddAsync(
            clinicalNote,
            cancellationToken);

        return Result<ClinicalNoteId>.Success(
            clinicalNote.Id);
    }
}