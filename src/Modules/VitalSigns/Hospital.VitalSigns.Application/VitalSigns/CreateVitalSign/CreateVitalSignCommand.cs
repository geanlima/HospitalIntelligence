namespace Hospital.VitalSigns.Application.VitalSigns.CreateVitalSign;

public sealed record CreateVitalSignCommand(
    Guid PatientId,
    DateTimeOffset MeasuredAtUtc,
    decimal? Temperature,
    int? HeartRate,
    int? RespiratoryRate,
    int? SystolicBloodPressure,
    int? DiastolicBloodPressure,
    decimal? OxygenSaturation);