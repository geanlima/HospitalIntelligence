using Hospital.Timeline.Domain.Timeline;
using Xunit;

namespace Hospital.Timeline.UnitTests.Timeline;

public sealed class TimelineItemTests
{
    [Fact]
    public void Create_ShouldCreateTimelineItem()
    {
        var patientId = Guid.NewGuid();
        var occurredAtUtc = DateTimeOffset.UtcNow;

        var item = TimelineItem.Create(
            patientId,
            occurredAtUtc,
            "Admission",
            "Paciente internado",
            "Paciente admitido na UTI.");

        Assert.NotEqual(Guid.Empty, item.Id.Value);
        Assert.Equal(patientId, item.PatientId);
        Assert.Equal(occurredAtUtc, item.OccurredAtUtc);
        Assert.Equal("Admission", item.Type);
        Assert.Equal("Paciente internado", item.Title);
        Assert.Equal(
            "Paciente admitido na UTI.",
            item.Description);
    }

    [Fact]
    public void Create_ShouldTrimTextFields()
    {
        var item = TimelineItem.Create(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            "  Exam  ",
            "  Exame realizado  ",
            "  Hemograma concluído.  ");

        Assert.Equal("Exam", item.Type);
        Assert.Equal("Exame realizado", item.Title);
        Assert.Equal(
            "Hemograma concluído.",
            item.Description);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            TimelineItem.Create(
                Guid.Empty,
                DateTimeOffset.UtcNow,
                "Admission",
                "Internação",
                "Paciente internado."));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidType_ShouldThrow(
        string type)
    {
        Assert.Throws<ArgumentException>(() =>
            TimelineItem.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                type,
                "Internação",
                "Paciente internado."));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ShouldThrow(
        string title)
    {
        Assert.Throws<ArgumentException>(() =>
            TimelineItem.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "Admission",
                title,
                "Paciente internado."));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidDescription_ShouldThrow(
        string description)
    {
        Assert.Throws<ArgumentException>(() =>
            TimelineItem.Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "Admission",
                "Internação",
                description));
    }
}