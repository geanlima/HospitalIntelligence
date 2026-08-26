using System.Collections.Concurrent;
using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Audit;

public sealed class InMemoryAiAuditStore : IAiAuditStore
{
    private readonly ConcurrentQueue<AiInteractionRecord> _records = new();

    public IReadOnlyCollection<AiInteractionRecord> Snapshot =>
        _records.ToArray();

    public Task SaveAsync(
        AiInteractionRecord record,
        CancellationToken cancellationToken = default)
    {
        _records.Enqueue(record);
        return Task.CompletedTask;
    }
}
