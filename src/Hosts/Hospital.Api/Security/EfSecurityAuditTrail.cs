using Hospital.Api.Security.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Security;

public sealed class EfSecurityAuditTrail : ISecurityAuditTrail
{
    private readonly SecurityDbContext _db;

    public EfSecurityAuditTrail(SecurityDbContext db)
    {
        _db = db;
    }

    public async Task AppendAsync(
        AuditTrailEntry entry,
        CancellationToken cancellationToken = default)
    {
        _db.AuditEntries.Add(
            new SecurityAuditEntryEntity
            {
                Id = entry.Id,
                OccurredAtUtc = entry.OccurredAtUtc,
                UserName = entry.UserName,
                Action = entry.Action,
                Resource = entry.Resource,
                CorrelationId = entry.CorrelationId,
                Details = entry.Details
            });

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditTrailEntry>> ListAsync(
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var rows = await _db.AuditEntries
            .AsNoTracking()
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(Math.Clamp(take, 1, 500))
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new AuditTrailEntry(
                x.Id,
                x.OccurredAtUtc,
                x.UserName,
                x.Action,
                x.Resource,
                x.CorrelationId,
                x.Details))
            .ToList();
    }
}
