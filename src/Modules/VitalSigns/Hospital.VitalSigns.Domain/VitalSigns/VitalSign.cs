using Hospital.SharedKernel.Domain;

namespace Hospital.VitalSigns.Domain.VitalSigns;

public sealed class VitalSign
    : AggregateRoot<VitalSignId>
{
    private VitalSign()
        : base(default)
    {
    }

    private VitalSign(
        VitalSignId id,
        Guid patientId,
        DateTimeOffset measuredAtUtc,
        decimal? temperature,
        int? heartRate,
        int? respiratoryRate,
        int? systolicBloodPressure,
        int? diastolicBloodPressure,
        decimal? oxygenSaturation)
        : base(id)
    {
        PatientId = patientId;
        MeasuredAtUtc = measuredAtUtc;
        Temperature = temperature;
        HeartRate = heartRate;
        RespiratoryRate = respiratoryRate;
        SystolicBloodPressure = systolicBloodPressure;
        DiastolicBloodPressure = diastolicBloodPressure;
        OxygenSaturation = oxygenSaturation;
    }

    public Guid PatientId { get; private set; }

    public DateTimeOffset MeasuredAtUtc { get; private set; }

    public decimal? Temperature { get; private set; }

    public int? HeartRate { get; private set; }

    public int? RespiratoryRate { get; private set; }

    public int? SystolicBloodPressure { get; private set; }

    public int? DiastolicBloodPressure { get; private set; }

    public decimal? OxygenSaturation { get; private set; }

    public static VitalSign Create(
        Guid patientId,
        DateTimeOffset measuredAtUtc,
        decimal? temperature,
        int? heartRate,
        int? respiratoryRate,
        int? systolicBloodPressure,
        int? diastolicBloodPressure,
        decimal? oxygenSaturation)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (temperature is null
            && heartRate is null
            && respiratoryRate is null
            && systolicBloodPressure is null
            && diastolicBloodPressure is null
            && oxygenSaturation is null)
        {
            throw new ArgumentException(
                "At least one vital sign value must be informed.");
        }

        if (temperature is < 25 or > 50)
        {
            throw new ArgumentOutOfRangeException(
                nameof(temperature),
                "Temperature must be between 25 and 50 degrees Celsius.");
        }

        if (heartRate is <= 0 or > 300)
        {
            throw new ArgumentOutOfRangeException(
                nameof(heartRate),
                "Heart rate must be between 1 and 300 bpm.");
        }

        if (respiratoryRate is <= 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(respiratoryRate),
                "Respiratory rate must be between 1 and 100 rpm.");
        }

        if (systolicBloodPressure is <= 0 or > 300)
        {
            throw new ArgumentOutOfRangeException(
                nameof(systolicBloodPressure),
                "Systolic blood pressure must be between 1 and 300 mmHg.");
        }

        if (diastolicBloodPressure is <= 0 or > 200)
        {
            throw new ArgumentOutOfRangeException(
                nameof(diastolicBloodPressure),
                "Diastolic blood pressure must be between 1 and 200 mmHg.");
        }

        if (oxygenSaturation is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(oxygenSaturation),
                "Oxygen saturation must be between 0 and 100 percent.");
        }

        if (systolicBloodPressure.HasValue
            && diastolicBloodPressure.HasValue
            && systolicBloodPressure <= diastolicBloodPressure)
        {
            throw new ArgumentException(
                "Systolic blood pressure must be greater than diastolic blood pressure.");
        }

        return new VitalSign(
            VitalSignId.New(),
            patientId,
            measuredAtUtc,
            temperature,
            heartRate,
            respiratoryRate,
            systolicBloodPressure,
            diastolicBloodPressure,
            oxygenSaturation);
    }
}