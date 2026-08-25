namespace Hospital.Patients.Contracts.Patient360;

public sealed record AdmissionSummaryResponse(
    Guid Id,
    DateTimeOffset AdmissionDate,
    DateTimeOffset? DischargeDate,
    string? Unit,
    string? Bed,
    string Status);