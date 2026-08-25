using Hospital.Alerts.Domain.Alerts;
using Xunit;

namespace Hospital.Alerts.UnitTests.Alerts;

public sealed class PatientAlertTests
{
    [Fact]
    public void Create_ShouldCreateActiveAlert()
    {
        var patientId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;

        var alert = PatientAlert.Create(
            patientId,
            "LowOxygenSaturation",
            AlertSeverity.High,
            "Saturação de oxigênio abaixo do limite esperado.",
            createdAtUtc);

        Assert.NotEqual(Guid.Empty, alert.Id.Value);
        Assert.Equal(patientId, alert.PatientId);
        Assert.Equal("LowOxygenSaturation", alert.Type);
        Assert.Equal(AlertSeverity.High, alert.Severity);
        Assert.Equal(
            "Saturação de oxigênio abaixo do limite esperado.",
            alert.Description);
        Assert.Equal(createdAtUtc, alert.CreatedAtUtc);
        Assert.Equal(AlertStatus.Active, alert.Status);
        Assert.Null(alert.AcknowledgedAtUtc);
        Assert.Null(alert.ResolvedAtUtc);
    }

    [Fact]
    public void Create_ShouldTrimTypeAndDescription()
    {
        var alert = PatientAlert.Create(
            Guid.NewGuid(),
            "  Fever  ",
            AlertSeverity.Medium,
            "  Temperatura elevada.  ",
            DateTimeOffset.UtcNow);

        Assert.Equal("Fever", alert.Type);
        Assert.Equal("Temperatura elevada.", alert.Description);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            PatientAlert.Create(
                Guid.Empty,
                "Fever",
                AlertSeverity.Medium,
                "Temperatura elevada.",
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidType_ShouldThrow(
        string type)
    {
        Assert.Throws<ArgumentException>(() =>
            PatientAlert.Create(
                Guid.NewGuid(),
                type,
                AlertSeverity.Medium,
                "Descrição do alerta.",
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
            PatientAlert.Create(
                Guid.NewGuid(),
                "Fever",
                AlertSeverity.Medium,
                description,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_WithInvalidSeverity_ShouldThrow()
    {
        var invalidSeverity = (AlertSeverity)999;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PatientAlert.Create(
                Guid.NewGuid(),
                "Fever",
                invalidSeverity,
                "Temperatura elevada.",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Acknowledge_ShouldChangeStatusToAcknowledged()
    {
        var createdAtUtc = DateTimeOffset.UtcNow.AddMinutes(-10);

        var alert = CreateAlert(createdAtUtc);

        var acknowledgedAtUtc = DateTimeOffset.UtcNow;

        alert.Acknowledge(acknowledgedAtUtc);

        Assert.Equal(
            AlertStatus.Acknowledged,
            alert.Status);

        Assert.Equal(
            acknowledgedAtUtc,
            alert.AcknowledgedAtUtc);
    }

    [Fact]
    public void Acknowledge_WithDateBeforeCreation_ShouldThrow()
    {
        var createdAtUtc = DateTimeOffset.UtcNow;

        var alert = CreateAlert(createdAtUtc);

        Assert.Throws<ArgumentException>(() =>
            alert.Acknowledge(
                createdAtUtc.AddMinutes(-1)));
    }

    [Fact]
    public void Acknowledge_WhenAlreadyAcknowledged_ShouldThrow()
    {
        var alert = CreateAlert(
            DateTimeOffset.UtcNow.AddMinutes(-10));

        alert.Acknowledge(
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            alert.Acknowledge(
                DateTimeOffset.UtcNow.AddMinutes(1)));
    }

    [Fact]
    public void Resolve_FromActive_ShouldChangeStatusToResolved()
    {
        var alert = CreateAlert(
            DateTimeOffset.UtcNow.AddMinutes(-10));

        var resolvedAtUtc = DateTimeOffset.UtcNow;

        alert.Resolve(resolvedAtUtc);

        Assert.Equal(
            AlertStatus.Resolved,
            alert.Status);

        Assert.Equal(
            resolvedAtUtc,
            alert.ResolvedAtUtc);
    }

    [Fact]
    public void Resolve_FromAcknowledged_ShouldChangeStatusToResolved()
    {
        var createdAtUtc =
            DateTimeOffset.UtcNow.AddMinutes(-20);

        var alert = CreateAlert(createdAtUtc);

        alert.Acknowledge(
            createdAtUtc.AddMinutes(5));

        var resolvedAtUtc =
            createdAtUtc.AddMinutes(10);

        alert.Resolve(resolvedAtUtc);

        Assert.Equal(
            AlertStatus.Resolved,
            alert.Status);

        Assert.Equal(
            resolvedAtUtc,
            alert.ResolvedAtUtc);
    }

    [Fact]
    public void Resolve_WhenAlreadyResolved_ShouldThrow()
    {
        var alert = CreateAlert(
            DateTimeOffset.UtcNow.AddMinutes(-10));

        alert.Resolve(
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() =>
            alert.Resolve(
                DateTimeOffset.UtcNow.AddMinutes(1)));
    }

    [Fact]
    public void Resolve_WithDateBeforeCreation_ShouldThrow()
    {
        var createdAtUtc = DateTimeOffset.UtcNow;

        var alert = CreateAlert(createdAtUtc);

        Assert.Throws<ArgumentException>(() =>
            alert.Resolve(
                createdAtUtc.AddMinutes(-1)));
    }

    private static PatientAlert CreateAlert(
        DateTimeOffset createdAtUtc)
    {
        return PatientAlert.Create(
            Guid.NewGuid(),
            "LowOxygenSaturation",
            AlertSeverity.High,
            "Saturação de oxigênio abaixo do limite esperado.",
            createdAtUtc);
    }
}