using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.SynchronizeExternalPatient;

public sealed record SynchronizeExternalPatientCommand(
    string SourceSystem,
    string ExternalId,
    string Name,
    DateOnly BirthDate,
    Gender Gender);