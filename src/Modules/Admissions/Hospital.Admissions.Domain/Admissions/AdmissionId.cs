namespace Hospital.Admissions.Domain.Admissions;

public readonly record struct AdmissionId(Guid Value)
{
    public static AdmissionId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}