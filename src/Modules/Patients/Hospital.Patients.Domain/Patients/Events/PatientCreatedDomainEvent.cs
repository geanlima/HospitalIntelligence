using Hospital.SharedKernel.Domain;

namespace Hospital.Patients.Domain.Patients.Events;

public sealed record PatientCreatedDomainEvent(
    PatientId PatientId,
    DateTimeOffset OccurredOnUtc)
    : IDomainEvent;