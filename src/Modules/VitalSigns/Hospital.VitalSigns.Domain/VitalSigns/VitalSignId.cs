namespace Hospital.VitalSigns.Domain.VitalSigns;

public readonly record struct VitalSignId(Guid Value)
{
    public static VitalSignId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}