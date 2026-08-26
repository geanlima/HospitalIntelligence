namespace Hospital.Admissions.Application.Admissions.SearchAdmissions;

public sealed record AdmissionResponse(
    Guid Id,
    Guid PatientId,
    DateTimeOffset AdmissionDate,
    DateTimeOffset? DischargeDate,
    string? Unit,
    string? Bed,
    string Status);