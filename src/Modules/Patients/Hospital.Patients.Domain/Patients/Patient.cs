using Hospital.Patients.Domain.Patients.Events;
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
        ValidateExternalSource(
            externalId,
            sourceSystem);

        var now = DateTimeOffset.UtcNow;

        var patient = new Patient(
            PatientId.New(),
            name.Trim(),
            birthDate,
            gender,
            externalId?.Trim(),
            sourceSystem?.Trim());

        patient.RaiseDomainEvent(
            new PatientCreatedDomainEvent(
                patient.Id,
                now));

        return patient;
    }

    public void ChangeName(string name)
    {
        ValidateName(name);

        var normalizedName = name.Trim();

        if (Name == normalizedName)
        {
            return;
        }

        Name = normalizedName;
        Touch();
    }

    public void ChangeBirthDate(DateOnly birthDate)
    {
        ValidateBirthDate(birthDate);

        if (BirthDate == birthDate)
        {
            return;
        }

        BirthDate = birthDate;
        Touch();
    }

    public void ChangeGender(Gender gender)
    {
        if (Gender == gender)
        {
            return;
        }

        Gender = gender;
        Touch();
    }

    public void UpdateExternalSource(
        string? externalId,
        string? sourceSystem)
    {
        ValidateExternalSource(
            externalId,
            sourceSystem);

        var normalizedExternalId =
            externalId?.Trim();

        var normalizedSourceSystem =
            sourceSystem?.Trim();

        if (ExternalId == normalizedExternalId &&
            SourceSystem == normalizedSourceSystem)
        {
            return;
        }

        ExternalId = normalizedExternalId;
        SourceSystem = normalizedSourceSystem;

        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new PatientDomainException(
                "Patient name cannot be empty.");
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length < 2)
        {
            throw new PatientDomainException(
                "Patient name must have at least 2 characters.");
        }

        if (normalizedName.Length > 200)
        {
            throw new PatientDomainException(
                "Patient name cannot exceed 200 characters.");
        }
    }

    private static void ValidateBirthDate(
        DateOnly birthDate)
    {
        var today =
            DateOnly.FromDateTime(
                DateTime.UtcNow);

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
            !string.IsNullOrWhiteSpace(
                externalId);

        var hasSourceSystem =
            !string.IsNullOrWhiteSpace(
                sourceSystem);

        if (hasExternalId != hasSourceSystem)
        {
            throw new PatientDomainException(
                "ExternalId and SourceSystem must be provided together.");
        }

        if (hasExternalId &&
            externalId!.Trim().Length > 100)
        {
            throw new PatientDomainException(
                "ExternalId cannot exceed 100 characters.");
        }

        if (hasSourceSystem &&
            sourceSystem!.Trim().Length > 50)
        {
            throw new PatientDomainException(
                "SourceSystem cannot exceed 50 characters.");
        }
    }
}