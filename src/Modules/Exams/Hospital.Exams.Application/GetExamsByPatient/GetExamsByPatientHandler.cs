using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Exams.Domain.Exams;

namespace Hospital.Exams.Application.Exams.GetExamsByPatient;

public sealed class GetExamsByPatientHandler
{
    private readonly IExamRepository _repository;

    public GetExamsByPatientHandler(
        IExamRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Exam>> HandleAsync(
        GetExamsByPatientQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetByPatientIdAsync(
            query.PatientId,
            cancellationToken);
    }
}