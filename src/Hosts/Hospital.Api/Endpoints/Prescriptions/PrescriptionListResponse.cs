namespace Hospital.Api.Endpoints.Prescriptions;

public sealed record PrescriptionListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string Description,
    DateTimeOffset PrescribedAtUtc,
    string Status);
