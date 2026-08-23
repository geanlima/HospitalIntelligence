using Hospital.Integration.Abstractions;
using Hospital.Integration.Messaging;
using Hospital.MockHospital.Contracts;
using Hospital.MockHospital.Mappers;

namespace Hospital.MockHospital.Adapters;

public sealed class MockHospitalAdapter
    : IExternalSystemAdapter
{
    private readonly Queue<MockHospitalPatientMessage> _messages = new();
    private readonly MockHospitalPatientMapper _mapper;

    public MockHospitalAdapter(
        MockHospitalPatientMapper mapper)
    {
        _mapper = mapper;
    }

    public string SourceSystem => "MOCK_HOSPITAL";

    public Task<IntegrationMessage> ReceiveAsync(
        CancellationToken cancellationToken = default)
    {
        if (_messages.Count == 0)
        {
            throw new InvalidOperationException(
                "No messages available.");
        }

        var externalMessage =
            _messages.Dequeue();

        var integrationMessage =
            _mapper.Map(externalMessage);

        return Task.FromResult(
            integrationMessage);
    }

    public void Enqueue(
        MockHospitalPatientMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        _messages.Enqueue(message);
    }
}