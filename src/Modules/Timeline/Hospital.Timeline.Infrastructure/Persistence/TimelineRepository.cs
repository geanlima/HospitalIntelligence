using Hospital.Timeline.Application.Timeline.Abstractions;
using Hospital.Timeline.Domain.Timeline;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Timeline.Infrastructure.Persistence;

public sealed class TimelineRepository
    : ITimelineRepository
{
    private readonly TimelineDbContext _dbContext;

    public TimelineRepository(
        TimelineDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TimelineItem?> GetByIdAsync(
        TimelineItemId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.TimelineItems
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        TimelineItem timelineItem,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.TimelineItems.AddAsync(
            timelineItem,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<TimelineItem>>
        GetByPatientIdAsync(
            Guid patientId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.TimelineItems
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.OccurredAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TimelineItem>> GetRecentAsync(
        int quantity,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.TimelineItems
            .AsNoTracking()
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(quantity)
            .ToListAsync(cancellationToken);
    }
}