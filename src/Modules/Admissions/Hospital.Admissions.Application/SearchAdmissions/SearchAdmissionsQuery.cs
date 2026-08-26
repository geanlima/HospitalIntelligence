using Hospital.Admissions.Domain.Admissions;

namespace Hospital.Admissions.Application.Admissions.SearchAdmissions;

public sealed record SearchAdmissionsQuery(
    AdmissionStatus? Status,
    string? Unit);