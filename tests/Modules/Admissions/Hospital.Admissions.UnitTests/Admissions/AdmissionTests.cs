using Hospital.Admissions.Domain.Admissions;
using Xunit;

namespace Hospital.Admissions.UnitTests.Admissions;

public sealed class AdmissionTests
{
    [Fact]
    public void Create_ShouldCreateActiveAdmission()
    {
        var patientId = Guid.NewGuid();

        var admissionDate =
            new DateTimeOffset(
                2026,
                8,
                24,
                10,
                0,
                0,
                TimeSpan.Zero);

        var admission =
            Admission.Create(
                patientId,
                admissionDate,
                "UTI",
                "101");

        Assert.NotEqual(
            Guid.Empty,
            admission.Id.Value);

        Assert.Equal(
            patientId,
            admission.PatientId);

        Assert.Equal(
            admissionDate,
            admission.AdmissionDate);

        Assert.Equal(
            "UTI",
            admission.Unit);

        Assert.Equal(
            "101",
            admission.Bed);

        Assert.Equal(
            AdmissionStatus.Active,
            admission.Status);

        Assert.Null(
            admission.DischargeDate);
    }

    [Fact]
    public void Create_ShouldTrimUnitAndBed()
    {
        var admission =
            Admission.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "  UTI  ",
                "  101  ");

        Assert.Equal(
            "UTI",
            admission.Unit);

        Assert.Equal(
            "101",
            admission.Bed);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () =>
                Admission.Create(
                    Guid.Empty,
                    DateTimeOffset.UtcNow,
                    "UTI",
                    "101"));
    }

    [Fact]
    public void Discharge_ShouldDischargeActiveAdmission()
    {
        var admissionDate =
            DateTimeOffset.UtcNow.AddDays(-2);

        var dischargeDate =
            DateTimeOffset.UtcNow;

        var admission =
            Admission.Create(
                Guid.NewGuid(),
                admissionDate,
                "Enfermaria",
                "202");

        admission.Discharge(
            dischargeDate);

        Assert.Equal(
            AdmissionStatus.Discharged,
            admission.Status);

        Assert.Equal(
            dischargeDate,
            admission.DischargeDate);
    }

    [Fact]
    public void Discharge_WithDateBeforeAdmission_ShouldThrow()
    {
        var admissionDate =
            DateTimeOffset.UtcNow;

        var admission =
            Admission.Create(
                Guid.NewGuid(),
                admissionDate,
                "Enfermaria",
                "202");

        var dischargeDate =
            admissionDate.AddHours(-1);

        Assert.Throws<ArgumentException>(
            () =>
                admission.Discharge(
                    dischargeDate));
    }

    [Fact]
    public void Discharge_WhenAlreadyDischarged_ShouldThrow()
    {
        var admission =
            Admission.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow.AddDays(-2),
                "Enfermaria",
                "202");

        admission.Discharge(
            DateTimeOffset.UtcNow.AddDays(-1));

        Assert.Throws<InvalidOperationException>(
            () =>
                admission.Discharge(
                    DateTimeOffset.UtcNow));
    }

    [Fact]
    public void ChangeLocation_ShouldUpdateUnitAndBed()
    {
        var admission =
            Admission.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "UTI",
                "101");

        admission.ChangeLocation(
            "Enfermaria",
            "305");

        Assert.Equal(
            "Enfermaria",
            admission.Unit);

        Assert.Equal(
            "305",
            admission.Bed);
    }

    [Fact]
    public void ChangeLocation_ShouldTrimValues()
    {
        var admission =
            Admission.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "UTI",
                "101");

        admission.ChangeLocation(
            "  Enfermaria  ",
            "  305  ");

        Assert.Equal(
            "Enfermaria",
            admission.Unit);

        Assert.Equal(
            "305",
            admission.Bed);
    }
}