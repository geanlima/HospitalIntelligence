namespace Hospital.Admissions.Application.Admissions.CreateAdmission;

public sealed record CreateAdmissionCommand(
    Guid PatientId,
    DateTimeOffset AdmissionDate,
    string? Unit,
    string? Bed);