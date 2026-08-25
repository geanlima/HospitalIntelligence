using Hospital.Prescriptions.Domain.Prescriptions;

namespace Hospital.Prescriptions.Application.ChangePrescriptionStatus;

public sealed record ChangePrescriptionStatusCommand(
    PrescriptionId PrescriptionId,
    PrescriptionStatus Status);