using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Application.Patients.Mappings;
using Hospital.Patients.Contracts.Patients;
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

    public async Task<Result<IReadOnlyCollection<PatientResponse>>> HandleAsync(
        SearchPatientsQuery query,
        CancellationToken cancellationToken = default)
    {
        var patients =
            await _patientRepository.SearchAsync(
                query.Name,
                cancellationToken);

        var response =
            patients
                .Select(x => x.ToResponse())
                .ToList()
                .AsReadOnly();

        return Result<IReadOnlyCollection<PatientResponse>>
            .Success(response);
    }
}