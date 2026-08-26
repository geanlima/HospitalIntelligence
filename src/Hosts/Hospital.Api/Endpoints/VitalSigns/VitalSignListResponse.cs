namespace Hospital.Api.Endpoints.VitalSigns;

public sealed record VitalSignListResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    DateTimeOffset MeasuredAtUtc,
    decimal? Temperature,
    int? HeartRate,
    int? RespiratoryRate,
    int? SystolicBloodPressure,
    int? DiastolicBloodPressure,
    decimal? OxygenSaturation);
