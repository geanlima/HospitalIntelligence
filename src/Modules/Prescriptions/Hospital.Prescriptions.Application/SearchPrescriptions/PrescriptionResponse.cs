namespace Hospital.Prescriptions.Application.SearchPrescriptions;

public sealed record PrescriptionResponse(
    Guid Id,
    Guid PatientId,
    string Description,
    DateTimeOffset PrescribedAtUtc,
    string Status);
