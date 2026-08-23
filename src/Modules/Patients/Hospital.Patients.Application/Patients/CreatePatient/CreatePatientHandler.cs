using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.SharedKernel.Application;

namespace Hospital.Patients.Application.Patients.CreatePatient;

public sealed class CreatePatientHandler
{
    private readonly IPatientRepository _patientRepository;

    public CreatePatientHandler(
        IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientId>> HandleAsync(
        CreatePatientCommand command,
        CancellationToken cancellationToken = default)
    {
        ExternalPatientIdentifier? externalIdentifier = null;

        if (!string.IsNullOrWhiteSpace(command.SourceSystem) ||
            !string.IsNullOrWhiteSpace(command.ExternalId))
        {
            if (string.IsNullOrWhiteSpace(command.SourceSystem) ||
                string.IsNullOrWhiteSpace(command.ExternalId))
            {
                return Result<PatientId>.Failure(
                    new Error(
                        "Patient.ExternalIdentifier.Invalid",
                        "SourceSystem and ExternalId must be provided together."));
            }

            var existingPatient =
                await _patientRepository.GetByExternalIdAsync(
                    command.SourceSystem,
                    command.ExternalId,
                    cancellationToken);

            if (existingPatient is not null)
            {
                return Result<PatientId>.Failure(
                    new Error(
                        "Patient.ExternalIdentifier.AlreadyExists",
                        "A patient with this external identifier already exists."));
            }

            externalIdentifier =
                ExternalPatientIdentifier.Create(
                    command.SourceSystem,
                    command.ExternalId);
        }

        var patient = Patient.Create(
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
}