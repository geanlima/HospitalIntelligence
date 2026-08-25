using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.ClinicalNotes.Domain.ClinicalNotes;

namespace Hospital.ClinicalNotes.Application.ClinicalNotes.GetClinicalNotesByPatient;

public sealed class GetClinicalNotesByPatientHandler
{
    private readonly IClinicalNoteRepository _repository;

    public GetClinicalNotesByPatientHandler(
        IClinicalNoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ClinicalNote>> HandleAsync(
        GetClinicalNotesByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}