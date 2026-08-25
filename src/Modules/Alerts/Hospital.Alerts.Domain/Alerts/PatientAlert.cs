using Hospital.SharedKernel.Domain;

namespace Hospital.Alerts.Domain.Alerts;

public sealed class PatientAlert
    : AggregateRoot<PatientAlertId>
{
    private PatientAlert()
        : base(default)
    {
    }

    private PatientAlert(
        PatientAlertId id,
        Guid patientId,
        string type,
        AlertSeverity severity,
        string description,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        PatientId = patientId;
        Type = type;
        Severity = severity;
        Description = description;
        CreatedAtUtc = createdAtUtc;
        Status = AlertStatus.Active;
    }

    public Guid PatientId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public AlertSeverity Severity { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public AlertStatus Status { get; private set; }

    public DateTimeOffset? AcknowledgedAtUtc { get; private set; }

    public DateTimeOffset? ResolvedAtUtc { get; private set; }

    public static PatientAlert Create(
        Guid patientId,
        string type,
        AlertSeverity severity,
        string description,
        DateTimeOffset createdAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException(
                "Alert type is required.",
                nameof(type));
        }

        if (!Enum.IsDefined(severity))
        {
            throw new ArgumentOutOfRangeException(
                nameof(severity),
                "Alert severity is invalid.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Alert description is required.",
                nameof(description));
        }

        return new PatientAlert(
            PatientAlertId.New(),
            patientId,
            type.Trim(),
            severity,
            description.Trim(),
            createdAtUtc);
    }

    public void Acknowledge(
        DateTimeOffset acknowledgedAtUtc)
    {
        if (Status != AlertStatus.Active)
        {
            throw new InvalidOperationException(
                "Only active alerts can be acknowledged.");
        }

        if (acknowledgedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Acknowledgement date cannot be before alert creation date.",
                nameof(acknowledgedAtUtc));
        }

        Status = AlertStatus.Acknowledged;
        AcknowledgedAtUtc = acknowledgedAtUtc;
    }

    public void Resolve(
        DateTimeOffset resolvedAtUtc)
    {
        if (Status == AlertStatus.Resolved)
        {
            throw new InvalidOperationException(
                "Alert is already resolved.");
        }

        if (resolvedAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException(
                "Resolution date cannot be before alert creation date.",
                nameof(resolvedAtUtc));
        }

        Status = AlertStatus.Resolved;
        ResolvedAtUtc = resolvedAtUtc;
    }
}