using Hospital.Prescriptions.Application.Abstractions;
using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Prescriptions.Application.GetPrescriptionsByPatient;

public sealed class GetPrescriptionsByPatientHandler
{
    private readonly IPrescriptionRepository _repository;

    public GetPrescriptionsByPatientHandler(
        IPrescriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Prescription>> HandleAsync(
        GetPrescriptionsByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}