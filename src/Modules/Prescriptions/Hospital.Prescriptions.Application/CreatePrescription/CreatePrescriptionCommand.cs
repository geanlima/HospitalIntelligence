namespace Hospital.Prescriptions.Application.CreatePrescription;

public sealed record CreatePrescriptionCommand(
    Guid PatientId,
    string Description,
    DateTimeOffset PrescribedAtUtc);