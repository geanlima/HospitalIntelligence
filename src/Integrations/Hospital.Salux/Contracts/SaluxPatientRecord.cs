namespace Hospital.Salux.Contracts;

public sealed record SaluxPatientRecord(
    string PatientCode,
    string PatientName,
    DateOnly BirthDate,
    int GenderCode,
    DateTimeOffset UpdatedAtUtc = default);
