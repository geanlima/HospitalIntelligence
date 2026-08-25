using Hospital.Prescriptions.Domain.Prescriptions;
using Xunit;

namespace Hospital.Prescriptions.UnitTests.Prescriptions;

public sealed class PrescriptionTests
{
    [Fact]
    public void Create_ShouldCreateActivePrescription()
    {
        var patientId = Guid.NewGuid();
        var prescribedAtUtc = DateTimeOffset.UtcNow;

        var prescription = Prescription.Create(
            patientId,
            "Dipirona 500mg a cada 6 horas",
            prescribedAtUtc);

        Assert.NotEqual(Guid.Empty, prescription.Id.Value);
        Assert.Equal(patientId, prescription.PatientId);
        Assert.Equal(
            "Dipirona 500mg a cada 6 horas",
            prescription.Description);
        Assert.Equal(prescribedAtUtc, prescription.PrescribedAtUtc);
        Assert.Equal(
            PrescriptionStatus.Active,
            prescription.Status);
    }

    [Fact]
    public void Create_ShouldTrimDescription()
    {
        var prescription = Prescription.Create(
            Guid.NewGuid(),
            "  Dipirona 500mg  ",
            DateTimeOffset.UtcNow);

        Assert.Equal(
            "Dipirona 500mg",
            prescription.Description);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            Prescription.Create(
                Guid.Empty,
                "Dipirona 500mg",
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidDescription_ShouldThrow(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            Prescription.Create(
                Guid.NewGuid(),
                description,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Suspend_ShouldChangeStatusToSuspended()
    {
        var prescription = CreatePrescription();

        prescription.Suspend();

        Assert.Equal(
            PrescriptionStatus.Suspended,
            prescription.Status);
    }

    [Fact]
    public void Suspend_WhenNotActive_ShouldThrow()
    {
        var prescription = CreatePrescription();

        prescription.Suspend();

        Assert.Throws<InvalidOperationException>(
            () => prescription.Suspend());
    }

    [Fact]
    public void Reactivate_ShouldChangeStatusToActive()
    {
        var prescription = CreatePrescription();

        prescription.Suspend();
        prescription.Reactivate();

        Assert.Equal(
            PrescriptionStatus.Active,
            prescription.Status);
    }

    [Fact]
    public void Reactivate_WhenNotSuspended_ShouldThrow()
    {
        var prescription = CreatePrescription();

        Assert.Throws<InvalidOperationException>(
            () => prescription.Reactivate());
    }

    [Fact]
    public void Complete_FromActive_ShouldChangeStatusToCompleted()
    {
        var prescription = CreatePrescription();

        prescription.Complete();

        Assert.Equal(
            PrescriptionStatus.Completed,
            prescription.Status);
    }

    [Fact]
    public void Complete_FromSuspended_ShouldChangeStatusToCompleted()
    {
        var prescription = CreatePrescription();

        prescription.Suspend();
        prescription.Complete();

        Assert.Equal(
            PrescriptionStatus.Completed,
            prescription.Status);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldThrow()
    {
        var prescription = CreatePrescription();

        prescription.Complete();

        Assert.Throws<InvalidOperationException>(
            () => prescription.Complete());
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var prescription = CreatePrescription();

        prescription.Cancel();

        Assert.Equal(
            PrescriptionStatus.Cancelled,
            prescription.Status);
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrow()
    {
        var prescription = CreatePrescription();

        prescription.Complete();

        Assert.Throws<InvalidOperationException>(
            () => prescription.Cancel());
    }

    private static Prescription CreatePrescription()
    {
        return Prescription.Create(
            Guid.NewGuid(),
            "Dipirona 500mg a cada 6 horas",
            DateTimeOffset.UtcNow);
    }
}