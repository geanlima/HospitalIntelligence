using Hospital.Exams.Application.Exams.Abstractions;

namespace Hospital.Exams.Application.Exams.SearchExams;

public sealed class SearchExamsHandler
{
    private readonly IExamRepository _examRepository;

    public SearchExamsHandler(
        IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<IReadOnlyCollection<ExamResponse>> HandleAsync(
        SearchExamsQuery query,
        CancellationToken cancellationToken = default)
    {
        var exams =
            await _examRepository.SearchAsync(
                query.Status,
                query.Name,
                cancellationToken);

        return exams
            .Select(x => new ExamResponse(
                x.Id.Value,
                x.PatientId,
                x.Name,
                x.RequestedAtUtc,
                x.ResultedAtUtc,
                x.Status.ToString(),
                x.Result))
            .ToList()
            .AsReadOnly();
    }
}
