namespace Hospital.VitalSigns.Application.VitalSigns.SearchVitalSigns;

public sealed record VitalSignResponse(
    Guid Id,
    Guid PatientId,
    DateTimeOffset MeasuredAtUtc,
    decimal? Temperature,
    int? HeartRate,
    int? RespiratoryRate,
    int? SystolicBloodPressure,
    int? DiastolicBloodPressure,
    decimal? OxygenSaturation);
