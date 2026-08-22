using Hospital.SharedKernel.Domain;

namespace Hospital.Patients.Domain.Patients;

public sealed class Patient : AggregateRoot<PatientId>
{
    private Patient(
        PatientId id,
        string name,
        DateOnly birthDate,
        Gender gender,
        string? externalId,
        string? sourceSystem)
        : base(id)
    {
        Name = name;
        BirthDate = birthDate;
        Gender = gender;
        ExternalId = externalId;
        SourceSystem = sourceSystem;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private Patient()
        : base(default)
    {
        Name = string.Empty;
    }

    public string Name { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public Gender Gender { get; private set; }

    public string? ExternalId { get; private set; }

    public string? SourceSystem { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Patient Create(
        string name,
        DateOnly birthDate,
        Gender gender,
        string? externalId = null,
        string? sourceSystem = null)
    {
        ValidateName(name);
        ValidateBirthDate(birthDate);
        ValidateExternalSource(externalId, sourceSystem);

        return new Patient(
            PatientId.New(),
            name.Trim(),
            birthDate,
            gender,
            externalId?.Trim(),
            sourceSystem?.Trim());
    }

    public void ChangeName(string name)
    {
        ValidateName(name);

        Name = name.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new PatientDomainException(
                "Patient name cannot be empty.");
        }

        if (name.Trim().Length > 200)
        {
            throw new PatientDomainException(
                "Patient name cannot exceed 200 characters.");
        }
    }

    private static void ValidateBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (birthDate > today)
        {
            throw new PatientDomainException(
                "Patient birth date cannot be in the future.");
        }
    }

    private static void ValidateExternalSource(
        string? externalId,
        string? sourceSystem)
    {
        var hasExternalId =
            !string.IsNullOrWhiteSpace(externalId);

        var hasSourceSystem =
            !string.IsNullOrWhiteSpace(sourceSystem);

        if (hasExternalId != hasSourceSystem)
        {
            throw new PatientDomainException(
                "ExternalId and SourceSystem must be provided together.");
        }
    }
}