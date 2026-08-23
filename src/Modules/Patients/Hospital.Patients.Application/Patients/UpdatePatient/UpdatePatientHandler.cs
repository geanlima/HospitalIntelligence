using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.SharedKernel.Application;

namespace Hospital.Patients.Application.Patients.UpdatePatient;

public sealed class UpdatePatientHandler
{
    private readonly IPatientRepository _patientRepository;

    public UpdatePatientHandler(
        IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result> HandleAsync(
        UpdatePatientCommand command,
        CancellationToken cancellationToken = default)
    {
        var patient =
            await _patientRepository.GetByIdAsync(
                command.PatientId,
                cancellationToken);

        if (patient is null)
        {
            return Result.Failure(
                new Error(
                    "Patient.NotFound",
                    "Patient was not found."));
        }

        patient.ChangeName(
            command.Name);

        patient.ChangeBirthDate(
            command.BirthDate);

        patient.ChangeGender(
            command.Gender);

        await _patientRepository.UpdateAsync(
            patient,
            cancellationToken);

        return Result.Success();
    }
}