namespace Hospital.Patients.Domain.Patients;

public sealed record ExternalPatientIdentifier
{
    private ExternalPatientIdentifier(
        string sourceSystem,
        string externalId)
    {
        SourceSystem = sourceSystem;
        ExternalId = externalId;
    }

    public string SourceSystem { get; }

    public string ExternalId { get; }

    public static ExternalPatientIdentifier Create(
        string sourceSystem,
        string externalId)
    {
        if (string.IsNullOrWhiteSpace(sourceSystem))
            throw new PatientDomainException(
                "SourceSystem cannot be empty.");

        if (string.IsNullOrWhiteSpace(externalId))
            throw new PatientDomainException(
                "ExternalId cannot be empty.");

        var normalizedSourceSystem = sourceSystem.Trim();
        var normalizedExternalId = externalId.Trim();

        if (normalizedSourceSystem.Length > 50)
            throw new PatientDomainException(
                "SourceSystem cannot exceed 50 characters.");

        if (normalizedExternalId.Length > 100)
            throw new PatientDomainException(
                "ExternalId cannot exceed 100 characters.");

        return new ExternalPatientIdentifier(
            normalizedSourceSystem,
            normalizedExternalId);
    }

    public override string ToString()
    {
        return $"{SourceSystem}:{ExternalId}";
    }
}