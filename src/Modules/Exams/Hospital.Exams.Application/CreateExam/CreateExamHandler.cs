using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Exams.Domain.Exams;
using Hospital.SharedKernel.Application;

namespace Hospital.Exams.Application.Exams.CreateExam;

public sealed class CreateExamHandler
{
    private readonly IExamRepository _repository;

    public CreateExamHandler(
        IExamRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ExamId>> HandleAsync(
        CreateExamCommand command,
        CancellationToken cancellationToken = default)
    {
        var exam =
            Exam.Create(
                command.PatientId,
                command.Name,
                command.RequestedAtUtc);

        await _repository.AddAsync(
            exam,
            cancellationToken);

        return Result<ExamId>.Success(
            exam.Id);
    }
}