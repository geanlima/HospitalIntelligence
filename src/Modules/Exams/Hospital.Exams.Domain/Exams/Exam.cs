using Hospital.SharedKernel.Domain;

namespace Hospital.Exams.Domain.Exams;

public sealed class Exam
    : AggregateRoot<ExamId>
{
    private Exam()
        : base(default)
    {
    }

    private Exam(
        ExamId id,
        Guid patientId,
        string name,
        DateTimeOffset requestedAtUtc)
        : base(id)
    {
        PatientId = patientId;
        Name = name;
        RequestedAtUtc = requestedAtUtc;
        Status = ExamStatus.Requested;
    }

    public Guid PatientId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTimeOffset RequestedAtUtc { get; private set; }

    public DateTimeOffset? ResultedAtUtc { get; private set; }

    public ExamStatus Status { get; private set; }

    public string? Result { get; private set; }

    public static Exam Create(
        Guid patientId,
        string name,
        DateTimeOffset requestedAtUtc)
    {
        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "Patient id is required.",
                nameof(patientId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Exam name is required.",
                nameof(name));
        }

        return new Exam(
            ExamId.New(),
            patientId,
            name.Trim(),
            requestedAtUtc);
    }

    public void Start()
    {
        if (Status != ExamStatus.Requested)
        {
            throw new InvalidOperationException(
                "Only requested exams can be started.");
        }

        Status = ExamStatus.InProgress;
    }

    public void RegisterResult(
        string result,
        DateTimeOffset resultedAtUtc)
    {
        if (Status == ExamStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cancelled exams cannot receive results.");
        }

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new ArgumentException(
                "Exam result is required.",
                nameof(result));
        }

        if (resultedAtUtc < RequestedAtUtc)
        {
            throw new ArgumentException(
                "Result date cannot be before request date.",
                nameof(resultedAtUtc));
        }

        Result = result.Trim();
        ResultedAtUtc = resultedAtUtc;
        Status = ExamStatus.Resulted;
    }

    public void Cancel()
    {
        if (Status == ExamStatus.Resulted)
        {
            throw new InvalidOperationException(
                "Resulted exams cannot be cancelled.");
        }

        Status = ExamStatus.Cancelled;
    }
}