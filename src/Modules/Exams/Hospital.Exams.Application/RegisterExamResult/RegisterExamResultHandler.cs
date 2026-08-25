using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.Exams.Application.Exams.RegisterExamResult;

public sealed class RegisterExamResultHandler
{
    private readonly IExamRepository _repository;

    public RegisterExamResultHandler(
        IExamRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> HandleAsync(
        RegisterExamResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var exam =
            await _repository.GetByIdAsync(
                command.ExamId,
                cancellationToken);

        if (exam is null)
        {
            return Result.Failure(
                new Error(
                    "Exam.NotFound",
                    "Exam was not found."));
        }

        exam.RegisterResult(
            command.Result,
            command.ResultedAtUtc);

        await _repository.UpdateAsync(
            exam,
            cancellationToken);

        return Result.Success();
    }
}