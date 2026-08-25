using Hospital.SharedKernel.Application;
using Hospital.Timeline.Application.Timeline.Abstractions;
using Hospital.Timeline.Domain.Timeline;

namespace Hospital.Timeline.Application.Timeline.CreateTimelineItem;

public sealed class CreateTimelineItemHandler
{
    private readonly ITimelineRepository _repository;

    public CreateTimelineItemHandler(
        ITimelineRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TimelineItemId>> HandleAsync(
        CreateTimelineItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var timelineItem = TimelineItem.Create(
            command.PatientId,
            command.OccurredAtUtc,
            command.Type,
            command.Title,
            command.Description);

        await _repository.AddAsync(
            timelineItem,
            cancellationToken);

        return Result<TimelineItemId>.Success(
            timelineItem.Id);
    }
}