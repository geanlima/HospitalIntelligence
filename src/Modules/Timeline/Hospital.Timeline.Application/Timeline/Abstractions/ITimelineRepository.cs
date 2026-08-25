using Hospital.Timeline.Domain.Timeline;

namespace Hospital.Timeline.Application.Timeline.Abstractions;

public interface ITimelineRepository
{
    Task<TimelineItem?> GetByIdAsync(
        TimelineItemId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TimelineItem timelineItem,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TimelineItem>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}