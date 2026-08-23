using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.SharedKernel.Application;

namespace Hospital.Patients.Application.Patients.SearchPatients;

public sealed class SearchPatientsHandler
{
    private readonly IPatientRepository _patientRepository;

    public SearchPatientsHandler(
        IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<IReadOnlyCollection<Patient>>> HandleAsync(
        SearchPatientsQuery query,
        CancellationToken cancellationToken = default)
    {
        var patients =
            await _patientRepository.SearchAsync(
                query.Name,
                cancellationToken);

        return Result<IReadOnlyCollection<Patient>>.Success(
            patients);
    }
}