using Hospital.Patients.Contracts.Patients;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.Mappings;

internal static class PatientMappings
{
    public static PatientResponse ToResponse(
        this Patient patient)
    {
        return new PatientResponse(
            patient.Id.Value,
            patient.Name,
            patient.BirthDate,
            patient.Gender.ToString(),
            patient.ExternalIdentifier?.SourceSystem,
            patient.ExternalIdentifier?.ExternalId,
            patient.CreatedAtUtc,
            patient.UpdatedAtUtc);
    }
}