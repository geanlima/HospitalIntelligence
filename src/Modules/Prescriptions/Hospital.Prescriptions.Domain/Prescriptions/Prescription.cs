using Hospital.SharedKernel.Domain;

namespace Hospital.Prescriptions.Domain.Prescriptions;

public sealed class Prescription
    : AggregateRoot<PrescriptionId>
{
    private Prescription()
        : base(default)
    {
    }

    private Prescription(
        PrescriptionId id,
        Guid patientId,
        string description,
        DateTimeOffset prescribedAtUtc)
        : base(id)
    {
        PatientId = patientId;
        Description = description;
        PrescribedAtUtc = prescribedAtUtc;
        Status = PrescriptionStatus.Active;
    }

    public Guid PatientId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset PrescribedAtUtc { get; private set; }

    public PrescriptionStatus Status { get; private set; }

    public static Prescription Create(
        Guid patientId,
        string description,
        DateTimeOffset prescribedAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(
                "Prescription description is required.",
                nameof(description));
        }

        return new Prescription(
            PrescriptionId.New(),
            patientId,
            description.Trim(),
            prescribedAtUtc);
    }

    public void Suspend()
    {
        if (Status != PrescriptionStatus.Active)
        {
            throw new InvalidOperationException(
                "Only active prescriptions can be suspended.");
        }

        Status = PrescriptionStatus.Suspended;
    }

    public void Reactivate()
    {
        if (Status != PrescriptionStatus.Suspended)
        {
            throw new InvalidOperationException(
                "Only suspended prescriptions can be reactivated.");
        }

        Status = PrescriptionStatus.Active;
    }

    public void Complete()
    {
        if (Status is PrescriptionStatus.Completed
            or PrescriptionStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Prescription cannot be completed.");
        }

        Status = PrescriptionStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == PrescriptionStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed prescriptions cannot be cancelled.");
        }

        Status = PrescriptionStatus.Cancelled;
    }
}