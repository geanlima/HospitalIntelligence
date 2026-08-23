using Hospital.MockHospital.Adapters;
using Hospital.MockHospital.Contracts;
using Hospital.MockHospital.Mappers;

namespace Hospital.MockHospital.UnitTests.Adapters;

public sealed class MockHospitalAdapterTests
{
    [Fact]
    public void SourceSystem_ShouldReturnMockHospital()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var adapter =
            new MockHospitalAdapter(mapper);

        Assert.Equal(
            "MOCK_HOSPITAL",
            adapter.SourceSystem);
    }

    [Fact]
    public async Task ReceiveAsync_WhenMessageExists_ShouldReturnIntegrationMessage()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var adapter =
            new MockHospitalAdapter(mapper);

        var externalMessage =
            new MockHospitalPatientMessage(
                "PAC-1001",
                "Carlos Almeida",
                new DateOnly(1980, 8, 14),
                1);

        adapter.Enqueue(
            externalMessage);

        var result =
            await adapter.ReceiveAsync();

        Assert.NotNull(result);

        Assert.Equal(
            "MOCK_HOSPITAL",
            result.SourceSystem);

        Assert.Equal(
            "Patient",
            result.MessageType);

        Assert.NotEqual(
            Guid.Empty,
            result.MessageId);

        Assert.NotEqual(
            Guid.Empty,
            result.CorrelationId);
    }

    [Fact]
    public async Task ReceiveAsync_WhenQueueIsEmpty_ShouldThrow()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var adapter =
            new MockHospitalAdapter(mapper);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => adapter.ReceiveAsync());

        Assert.Equal(
            "No messages available.",
            exception.Message);
    }

    [Fact]
    public void Enqueue_WithNullMessage_ShouldThrow()
    {
        var mapper =
            new MockHospitalPatientMapper();

        var adapter =
            new MockHospitalAdapter(mapper);

        Assert.Throws<ArgumentNullException>(
            () => adapter.Enqueue(null!));
    }
}