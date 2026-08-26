using System.Text.Json;
using Hospital.Salux.Contracts;
using Hospital.Salux.Mappers;

namespace Hospital.Salux.UnitTests.Mappers;

public sealed class SaluxPatientMapperTests
{
    [Fact]
    public void Map_ShouldCreateIntegrationMessageWithExpectedValues()
    {
        var mapper =
            new SaluxPatientMapper();

        var patient =
            new SaluxPatientRecord(
                "SALUX-1001",
                "Paciente Salux",
                new DateOnly(1985, 5, 10),
                1);

        var result =
            mapper.Map(patient);

        Assert.NotEqual(
            Guid.Empty,
            result.MessageId);

        Assert.NotEqual(
            Guid.Empty,
            result.CorrelationId);

        Assert.Equal(
            "SALUX",
            result.SourceSystem);

        Assert.Equal(
            "Patient",
            result.MessageType);

        Assert.False(
            string.IsNullOrWhiteSpace(result.Payload));
    }

    [Fact]
    public void Map_ShouldSerializePatientInsidePayload()
    {
        var mapper =
            new SaluxPatientMapper();

        var patient =
            new SaluxPatientRecord(
                "SALUX-1002",
                "Maria Salux",
                new DateOnly(1990, 3, 20),
                2);

        var result =
            mapper.Map(patient);

        var payload =
            JsonSerializer.Deserialize<SaluxPatientRecord>(
                result.Payload);

        Assert.NotNull(payload);

        Assert.Equal(
            patient.PatientCode,
            payload.PatientCode);

        Assert.Equal(
            patient.PatientName,
            payload.PatientName);

        Assert.Equal(
            patient.BirthDate,
            payload.BirthDate);

        Assert.Equal(
            patient.GenderCode,
            payload.GenderCode);
    }

    [Fact]
    public void Map_ShouldCreateDeterministicMessageIdForSameVersion()
    {
        var mapper = new SaluxPatientMapper();
        var updatedAt = new DateTimeOffset(2026, 8, 26, 12, 0, 0, TimeSpan.Zero);

        var first = mapper.Map(
            new SaluxPatientRecord(
                "SALUX-1003",
                "Paciente",
                new DateOnly(1991, 1, 1),
                1,
                updatedAt));

        var second = mapper.Map(
            new SaluxPatientRecord(
                "SALUX-1003",
                "Paciente",
                new DateOnly(1991, 1, 1),
                1,
                updatedAt));

        Assert.Equal(first.MessageId, second.MessageId);
    }

    [Fact]
    public void Map_WithNullPatient_ShouldThrow()
    {
        var mapper =
            new SaluxPatientMapper();

        Assert.Throws<ArgumentNullException>(
            () => mapper.Map(null!));
    }
}