namespace Hospital.Patients.Contracts.IntegrationEvents;

public sealed record PatientCreatedIntegrationEvent(
    Guid PatientId,
    string Name,
    DateOnly BirthDate,
    string Gender,
    string? SourceSystem,
    string? ExternalId,
    DateTimeOffset OccurredOnUtc);