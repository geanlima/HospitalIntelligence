namespace Hospital.Patients.Domain.Patients;

public readonly record struct PatientId(Guid Value)
{
    public static PatientId New()
        => new(Guid.NewGuid());

    public override string ToString()
        => Value.ToString();
}