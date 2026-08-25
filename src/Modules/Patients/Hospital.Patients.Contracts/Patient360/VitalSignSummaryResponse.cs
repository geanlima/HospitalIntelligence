namespace Hospital.Patients.Contracts.Patient360;

public sealed record VitalSignSummaryResponse(
    Guid Id,
    DateTimeOffset MeasuredAtUtc,
    decimal? Temperature,
    int? HeartRate,
    int? RespiratoryRate,
    int? SystolicBloodPressure,
    int? DiastolicBloodPressure,
    decimal? OxygenSaturation);