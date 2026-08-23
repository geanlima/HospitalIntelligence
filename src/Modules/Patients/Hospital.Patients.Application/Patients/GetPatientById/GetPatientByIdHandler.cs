using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.SharedKernel.Application;

namespace Hospital.Patients.Application.Patients.GetPatientById;

public sealed class GetPatientByIdHandler
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientByIdHandler(
        IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<Patient>> HandleAsync(
        GetPatientByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var patient =
            await _patientRepository.GetByIdAsync(
                query.PatientId,
                cancellationToken);

        if (patient is null)
        {
            return Result<Patient>.Failure(
                new Error(
                    "Patient.NotFound",
                    "Patient was not found."));
        }

        return Result<Patient>.Success(patient);
    }
}