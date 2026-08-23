using Hospital.Integration.Messaging;

namespace Hospital.Integration.UnitTests.Messaging;

public sealed class MessageEnvelopeTests
{
    [Fact]
    public void Create_ShouldCreateEnvelopeWithExpectedValues()
    {
        var payload =
            new TestPayload(
                Guid.NewGuid(),
                "Paciente Teste");

        var envelope =
            MessageEnvelope<TestPayload>.Create(
                "SALUX",
                payload);

        Assert.NotEqual(
            Guid.Empty,
            envelope.MessageId);

        Assert.NotEqual(
            Guid.Empty,
            envelope.CorrelationId);

        Assert.Equal(
            "SALUX",
            envelope.SourceSystem);

        Assert.Equal(
            payload,
            envelope.Payload);

        Assert.True(
            envelope.CreatedAtUtc <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_WithCorrelationId_ShouldPreserveCorrelationId()
    {
        var correlationId =
            Guid.NewGuid();

        var payload =
            new TestPayload(
                Guid.NewGuid(),
                "Paciente Teste");

        var envelope =
            MessageEnvelope<TestPayload>.Create(
                "SALUX",
                payload,
                correlationId);

        Assert.Equal(
            correlationId,
            envelope.CorrelationId);
    }

    [Fact]
    public void Create_ShouldTrimSourceSystem()
    {
        var payload =
            new TestPayload(
                Guid.NewGuid(),
                "Paciente Teste");

        var envelope =
            MessageEnvelope<TestPayload>.Create(
                "  SALUX  ",
                payload);

        Assert.Equal(
            "SALUX",
            envelope.SourceSystem);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidSourceSystem_ShouldThrow(
        string sourceSystem)
    {
        var payload =
            new TestPayload(
                Guid.NewGuid(),
                "Paciente Teste");

        Assert.Throws<ArgumentException>(
            () =>
                MessageEnvelope<TestPayload>.Create(
                    sourceSystem,
                    payload));
    }

    [Fact]
    public void Create_WithNullPayload_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                MessageEnvelope<TestPayload>.Create(
                    "SALUX",
                    null!));
    }

    private sealed record TestPayload(
        Guid PatientId,
        string Name);
}