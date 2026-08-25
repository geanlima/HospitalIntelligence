using Hospital.SharedKernel.Domain;

namespace Hospital.Admissions.Domain.Admissions;

public sealed class Admission
    : AggregateRoot<AdmissionId>
{
    // Necessário para o Entity Framework Core.
    private Admission()
        : base(default)
    {
    }

    private Admission(
        AdmissionId id,
        Guid patientId,
        DateTimeOffset admissionDate,
        string? unit,
        string? bed)
        : base(id)
    {
        PatientId = patientId;
        AdmissionDate = admissionDate;
        Unit = unit;
        Bed = bed;
        Status = AdmissionStatus.Active;
    }

    public Guid PatientId { get; private set; }

    public DateTimeOffset AdmissionDate { get; private set; }

    public DateTimeOffset? DischargeDate { get; private set; }

    public string? Unit { get; private set; }

    public string? Bed { get; private set; }

    public AdmissionStatus Status { get; private set; }

    public static Admission Create(
        Guid patientId,
        DateTimeOffset admissionDate,
        string? unit,
        string? bed)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        return new Admission(
            AdmissionId.New(),
            patientId,
            admissionDate,
            unit?.Trim(),
            bed?.Trim());
    }

    public void Discharge(
        DateTimeOffset dischargeDate)
    {
        if (Status != AdmissionStatus.Active)
        {
            throw new InvalidOperationException(
                "Only active admissions can be discharged.");
        }

        if (dischargeDate < AdmissionDate)
        {
            throw new ArgumentException(
                "Discharge date cannot be before admission date.",
                nameof(dischargeDate));
        }

        DischargeDate = dischargeDate;
        Status = AdmissionStatus.Discharged;
    }

    public void ChangeLocation(
        string? unit,
        string? bed)
    {
        Unit = unit?.Trim();
        Bed = bed?.Trim();
    }
}