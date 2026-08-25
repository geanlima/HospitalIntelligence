using Hospital.Admissions.Domain.Admissions;

namespace Hospital.Admissions.Application.Admissions.DischargeAdmission;

public sealed record DischargeAdmissionCommand(
    AdmissionId AdmissionId,
    DateTimeOffset DischargeDate);