namespace Hospital.Api.Endpoints.Admissions;

public sealed record AdmissionListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    DateTimeOffset AdmissionDate,
    DateTimeOffset? DischargeDate,
    string? Unit,
    string? Bed,
    string Status);