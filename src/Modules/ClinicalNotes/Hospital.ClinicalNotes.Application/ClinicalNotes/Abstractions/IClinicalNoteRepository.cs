using Hospital.ClinicalNotes.Domain.ClinicalNotes;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;

public interface IClinicalNoteRepository
{
    Task<ClinicalNote?> GetByIdAsync(
        ClinicalNoteId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ClinicalNote clinicalNote,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ClinicalNote>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}