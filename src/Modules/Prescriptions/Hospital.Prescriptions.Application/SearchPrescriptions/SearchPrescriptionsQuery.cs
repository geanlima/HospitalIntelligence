using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Prescriptions.Application.SearchPrescriptions;

public sealed record SearchPrescriptionsQuery(
    PrescriptionStatus? Status);
