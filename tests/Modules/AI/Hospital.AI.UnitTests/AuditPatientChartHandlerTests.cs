using Hospital.AI.Application.Abstractions;
using Hospital.AI.Application.Audit;

namespace Hospital.AI.UnitTests;

public class AuditPatientChartHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_Findings_For_Incomplete_Chart()
    {
        var patientId = Guid.NewGuid();
        var source = new FakeClinicalRecordSource();
        source.AddRecord(
            new ClinicalRecordSnapshot(
                $"admission:{Guid.NewGuid()}",
                "Admission",
                "UTI",
                "Internação status Active.",
                patientId,
                DateTimeOffset.UtcNow.AddDays(-1),
                "Active"));

        var handler = new AuditPatientChartHandler(
            new AllowAllAiAccessPolicy(),
            source);

        var result = await handler.HandleAsync(
            new AuditPatientChartQuery(patientId));

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.Findings);
        Assert.True(result.Value.MissingDocumentationCount > 0);
        Assert.NotEqual("None", result.Value.OverallRisk);
    }

    [Fact]
    public async Task HandleAsync_Should_Deny_When_Access_Fails()
    {
        var handler = new AuditPatientChartHandler(
            new DenyAiAccessPolicy(),
            new FakeClinicalRecordSource());

        var result = await handler.HandleAsync(
            new AuditPatientChartQuery(Guid.NewGuid()));

        Assert.True(result.IsFailure);
        Assert.Equal("AI.Access.PatientNotFound", result.Error.Code);
    }
}
