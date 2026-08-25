using Hospital.ClinicalNotes.Domain.ClinicalNotes;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;

public sealed record CreateClinicalNoteCommand(
    Guid PatientId,
    string Professional,
    ClinicalNoteType NoteType,
    string Content,
    DateTimeOffset CreatedAtUtc);