using Hospital.Timeline.Application.Timeline.Abstractions;
using Hospital.Timeline.Domain.Timeline;

namespace Hospital.Timeline.Application.Timeline.GetTimelineByPatient;

public sealed class GetTimelineByPatientHandler
{
    private readonly ITimelineRepository _repository;

    public GetTimelineByPatientHandler(
        ITimelineRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<TimelineItem>> HandleAsync(
        GetTimelineByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}