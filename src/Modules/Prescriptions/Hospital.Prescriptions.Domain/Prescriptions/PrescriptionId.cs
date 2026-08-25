namespace Hospital.Prescriptions.Domain.Prescriptions;

public readonly record struct PrescriptionId(Guid Value)
{
    public static PrescriptionId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}