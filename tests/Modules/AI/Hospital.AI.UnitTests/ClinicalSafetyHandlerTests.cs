using Hospital.AI.Application.Abstractions;
using Hospital.AI.Application.ClinicalSafety;
using Hospital.AI.Infrastructure.Guardrails;
using Hospital.AI.Infrastructure.Prompts;
using Hospital.AI.Infrastructure.Providers;

namespace Hospital.AI.UnitTests;

public class ClinicalSafetyHandlerTests
{
    [Fact]
    public async Task Assess_Should_Return_Safety_Result()
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

        var handler = new AssessClinicalSafetyHandler(
            new AllowAllAiAccessPolicy(),
            source);

        var result = await handler.HandleAsync(
            new AssessClinicalSafetyQuery(patientId));

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.Summary));
        Assert.False(result.Value.DischargeReady);
    }

    [Fact]
    public async Task VoiceNote_Should_Structure_Transcript()
    {
        var handler = new StructureVoiceNoteHandler(
            new AllowAllAiAccessPolicy(),
            new BasicAiGuardrail(),
            new InMemoryPromptCatalog(),
            new MockLlmProvider());

        var result = await handler.HandleAsync(
            new StructureVoiceNoteCommand(
                "Paciente refere melhora da dispneia após oxigênio.",
                null,
                "Evolution"));

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.StructuredContent));
        Assert.Equal("Evolution", result.Value.NoteType);
    }
}
