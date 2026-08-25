using Hospital.VitalSigns.Application.VitalSigns.Abstractions;
using Hospital.VitalSigns.Domain.VitalSigns;

namespace Hospital.VitalSigns.Application.VitalSigns.GetVitalSignsByPatient;

public sealed class GetVitalSignsByPatientHandler
{
    private readonly IVitalSignRepository _repository;

    public GetVitalSignsByPatientHandler(
        IVitalSignRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<VitalSign>> HandleAsync(
        GetVitalSignsByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}