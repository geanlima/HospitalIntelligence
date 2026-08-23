using System.Text.Json;
using Hospital.MockHospital.Contracts;
using Hospital.MockHospital.Mappers;

namespace Hospital.MockHospital.UnitTests.Mappers;

public sealed class MockHospitalPatientMapperTests
{
    [Fact]
    public void Map_ShouldCreateIntegrationMessageWithExpectedValues()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var externalPatient =
            new MockHospitalPatientMessage(
                "PAC-1001",
                "Carlos Almeida",
                new DateOnly(1980, 8, 14),
                1);

        var result =
            mapper.Map(
                externalPatient);

        Assert.NotEqual(
            Guid.Empty,
            result.MessageId);

        Assert.NotEqual(
            Guid.Empty,
            result.CorrelationId);

        Assert.Equal(
            "MOCK_HOSPITAL",
            result.SourceSystem);

        Assert.Equal(
            "Patient",
            result.MessageType);

        Assert.False(
            string.IsNullOrWhiteSpace(result.Payload));
    }

    [Fact]
    public void Map_ShouldPreserveMessageIdAndCorrelationId()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var messageId =
            Guid.NewGuid();

        var correlationId =
            Guid.NewGuid();

        var externalPatient =
            new MockHospitalPatientMessage(
                "PAC-1002",
                "Maria Souza",
                new DateOnly(1990, 2, 15),
                2);

        var result =
            mapper.Map(
                externalPatient,
                messageId,
                correlationId);

        Assert.Equal(
            messageId,
            result.MessageId);

        Assert.Equal(
            correlationId,
            result.CorrelationId);
    }

    [Fact]
    public void Map_ShouldSerializePatientInsidePayload()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var externalPatient =
            new MockHospitalPatientMessage(
                "PAC-1003",
                "Ana Oliveira",
                new DateOnly(1988, 4, 20),
                2);

        var result =
            mapper.Map(
                externalPatient);

        var payload =
            JsonSerializer.Deserialize<
                MockHospitalPatientMessage>(
                result.Payload);

        Assert.NotNull(payload);

        Assert.Equal(
            externalPatient.ExternalId,
            payload.ExternalId);

        Assert.Equal(
            externalPatient.Name,
            payload.Name);

        Assert.Equal(
            externalPatient.BirthDate,
            payload.BirthDate);

        Assert.Equal(
            externalPatient.Gender,
            payload.Gender);
    }

    [Fact]
    public void Map_WithNullPatient_ShouldThrow()
    {
        var mapper =
            new MockHospitalPatientMapper();

        Assert.Throws<ArgumentNullException>(
            () => mapper.Map(null!));
    }
}