using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.SharedKernel.Application;

namespace Hospital.Patients.Application.Patients.SynchronizeExternalPatient;

public sealed class SynchronizeExternalPatientHandler
{
    private readonly IPatientRepository _patientRepository;

    public SynchronizeExternalPatientHandler(
        IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientId>> HandleAsync(
        SynchronizeExternalPatientCommand command,
        CancellationToken cancellationToken = default)
    {
        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                command.SourceSystem,
                command.ExternalId);

        var existingPatient =
            await _patientRepository.GetByExternalIdAsync(
                externalIdentifier.SourceSystem,
                externalIdentifier.ExternalId,
                cancellationToken);

        if (existingPatient is null)
        {
            var patient =
                Patient.Create(
                    command.Name,
                    command.BirthDate,
                    command.Gender,
                    externalIdentifier);

            await _patientRepository.AddAsync(
                patient,
                cancellationToken);

            return Result<PatientId>.Success(
                patient.Id);
        }

        existingPatient.ChangeName(
            command.Name);

        existingPatient.ChangeBirthDate(
            command.BirthDate);

        existingPatient.ChangeGender(
            command.Gender);

        await _patientRepository.UpdateAsync(
            existingPatient,
            cancellationToken);

        return Result<PatientId>.Success(
            existingPatient.Id);
    }
}