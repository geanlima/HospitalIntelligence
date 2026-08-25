using Hospital.Exams.Domain.Exams;
using Xunit;

namespace Hospital.Exams.UnitTests.Exams;

public sealed class ExamTests
{
    [Fact]
    public void Create_ShouldCreateRequestedExam()
    {
        var patientId = Guid.NewGuid();
        var requestedAtUtc = DateTimeOffset.UtcNow;

        var exam =
            Exam.Create(
                patientId,
                "Hemograma",
                requestedAtUtc);

        Assert.NotEqual(
            Guid.Empty,
            exam.Id.Value);

        Assert.Equal(
            patientId,
            exam.PatientId);

        Assert.Equal(
            "Hemograma",
            exam.Name);

        Assert.Equal(
            requestedAtUtc,
            exam.RequestedAtUtc);

        Assert.Equal(
            ExamStatus.Requested,
            exam.Status);

        Assert.Null(
            exam.Result);

        Assert.Null(
            exam.ResultedAtUtc);
    }

    [Fact]
    public void Create_ShouldTrimName()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "  Hemograma  ",
                DateTimeOffset.UtcNow);

        Assert.Equal(
            "Hemograma",
            exam.Name);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () =>
                Exam.Create(
                    Guid.Empty,
                    "Hemograma",
                    DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrow(
        string name)
    {
        Assert.Throws<ArgumentException>(
            () =>
                Exam.Create(
                    Guid.NewGuid(),
                    name,
                    DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Start_ShouldChangeStatusToInProgress()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow);

        exam.Start();

        Assert.Equal(
            ExamStatus.InProgress,
            exam.Status);
    }

    [Fact]
    public void Start_WhenAlreadyStarted_ShouldThrow()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow);

        exam.Start();

        Assert.Throws<InvalidOperationException>(
            () => exam.Start());
    }

    [Fact]
    public void RegisterResult_ShouldCompleteExam()
    {
        var requestedAtUtc =
            DateTimeOffset.UtcNow.AddHours(-2);

        var resultedAtUtc =
            DateTimeOffset.UtcNow;

        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                requestedAtUtc);

        exam.RegisterResult(
            "Resultado normal",
            resultedAtUtc);

        Assert.Equal(
            ExamStatus.Resulted,
            exam.Status);

        Assert.Equal(
            "Resultado normal",
            exam.Result);

        Assert.Equal(
            resultedAtUtc,
            exam.ResultedAtUtc);
    }

    [Fact]
    public void RegisterResult_ShouldTrimResult()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow.AddHours(-1));

        exam.RegisterResult(
            "  Resultado normal  ",
            DateTimeOffset.UtcNow);

        Assert.Equal(
            "Resultado normal",
            exam.Result);
    }

    [Fact]
    public void RegisterResult_WithDateBeforeRequest_ShouldThrow()
    {
        var requestedAtUtc =
            DateTimeOffset.UtcNow;

        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                requestedAtUtc);

        Assert.Throws<ArgumentException>(
            () =>
                exam.RegisterResult(
                    "Resultado",
                    requestedAtUtc.AddMinutes(-1)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void RegisterResult_WithInvalidResult_ShouldThrow(
        string result)
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow.AddHours(-1));

        Assert.Throws<ArgumentException>(
            () =>
                exam.RegisterResult(
                    result,
                    DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow);

        exam.Cancel();

        Assert.Equal(
            ExamStatus.Cancelled,
            exam.Status);
    }

    [Fact]
    public void RegisterResult_WhenCancelled_ShouldThrow()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow.AddHours(-1));

        exam.Cancel();

        Assert.Throws<InvalidOperationException>(
            () =>
                exam.RegisterResult(
                    "Resultado",
                    DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Cancel_WhenResulted_ShouldThrow()
    {
        var exam =
            Exam.Create(
                Guid.NewGuid(),
                "Hemograma",
                DateTimeOffset.UtcNow.AddHours(-1));

        exam.RegisterResult(
            "Resultado normal",
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => exam.Cancel());
    }
}