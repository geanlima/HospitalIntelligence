namespace Hospital.Patients.Domain.Patients;

public sealed class PatientDomainException : Exception
{
    public PatientDomainException(string message)
        : base(message)
    {
    }
}