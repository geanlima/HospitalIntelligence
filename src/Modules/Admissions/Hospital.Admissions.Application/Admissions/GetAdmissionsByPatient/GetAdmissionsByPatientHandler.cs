using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Admissions.Domain.Admissions;

namespace Hospital.Admissions.Application.Admissions.GetAdmissionsByPatient;

public sealed class GetAdmissionsByPatientHandler
{
    private readonly IAdmissionRepository _repository;

    public GetAdmissionsByPatientHandler(
        IAdmissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Admission>> HandleAsync(
        GetAdmissionsByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}