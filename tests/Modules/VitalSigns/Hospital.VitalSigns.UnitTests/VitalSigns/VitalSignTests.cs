using Hospital.VitalSigns.Domain.VitalSigns;
using Xunit;

namespace Hospital.VitalSigns.UnitTests.VitalSigns;

public sealed class VitalSignTests
{
    [Fact]
    public void Create_ShouldCreateVitalSign()
    {
        var patientId = Guid.NewGuid();
        var measuredAtUtc = DateTimeOffset.UtcNow;

        var vitalSign = VitalSign.Create(
            patientId,
            measuredAtUtc,
            36.7m,
            80,
            18,
            120,
            80,
            98m);

        Assert.NotEqual(Guid.Empty, vitalSign.Id.Value);
        Assert.Equal(patientId, vitalSign.PatientId);
        Assert.Equal(measuredAtUtc, vitalSign.MeasuredAtUtc);
        Assert.Equal(36.7m, vitalSign.Temperature);
        Assert.Equal(80, vitalSign.HeartRate);
        Assert.Equal(18, vitalSign.RespiratoryRate);
        Assert.Equal(120, vitalSign.SystolicBloodPressure);
        Assert.Equal(80, vitalSign.DiastolicBloodPressure);
        Assert.Equal(98m, vitalSign.OxygenSaturation);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            VitalSign.Create(
                Guid.Empty,
                DateTimeOffset.UtcNow,
                36.5m,
                null,
                null,
                null,
                null,
                null));
    }

    [Fact]
    public void Create_WithoutAnyValue_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData(24.9)]
    [InlineData(50.1)]
    public void Create_WithInvalidTemperature_ShouldThrow(
        double temperature)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                (decimal)temperature,
                null,
                null,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(301)]
    public void Create_WithInvalidHeartRate_ShouldThrow(
        int heartRate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                heartRate,
                null,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Create_WithInvalidRespiratoryRate_ShouldThrow(
        int respiratoryRate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                respiratoryRate,
                null,
                null,
                null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(301)]
    public void Create_WithInvalidSystolicPressure_ShouldThrow(
        int systolic)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                systolic,
                80,
                null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public void Create_WithInvalidDiastolicPressure_ShouldThrow(
        int diastolic)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                120,
                diastolic,
                null));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_WithInvalidOxygenSaturation_ShouldThrow(
        double oxygenSaturation)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null,
                null,
                (decimal)oxygenSaturation));
    }

    [Fact]
    public void Create_WithSystolicLowerThanDiastolic_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            VitalSign.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                80,
                120,
                null));
    }

    [Fact]
    public void Create_WithOnlyOneVitalSign_ShouldBeAllowed()
    {
        var vitalSign = VitalSign.Create(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            null,
            75,
            null,
            null,
            null,
            null);

        Assert.Equal(75, vitalSign.HeartRate);
    }
}