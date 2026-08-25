namespace Hospital.Alerts.Domain.Alerts;

public readonly record struct PatientAlertId(Guid Value)
{
    public static PatientAlertId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}