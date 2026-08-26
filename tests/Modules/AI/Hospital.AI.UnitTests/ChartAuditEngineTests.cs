using Hospital.AI.Application.Audit;

namespace Hospital.AI.UnitTests;

public class ChartAuditEngineTests
{
    [Fact]
    public void Evaluate_ActiveAdmissionWithoutNotes_FindsMissingDocsAndGlosa()
    {
        var now = DateTimeOffset.UtcNow;

        var records = new List<ClinicalRecordSnapshotProxy>
        {
            new(
                "admission:1",
                "Admission",
                "UTI",
                "Internação status Active.",
                now.AddDays(-1),
                "Active",
                null)
        };

        var findings = ChartAuditEngine.Evaluate(records, now);

        Assert.Contains(
            findings,
            f => f.Code == "DOC.NO_CLINICAL_NOTES");
        Assert.Contains(
            findings,
            f => f.Code == "DOC.NO_MEDICAL_EVOLUTION");
        Assert.Contains(
            findings,
            f => f.Code == "GLOSA.MISSING_DAILY_EVOLUTION");
        Assert.Contains(
            findings,
            f => f.Code == "DOC.NO_VITAL_SIGNS");
    }

    [Fact]
    public void Evaluate_LowSpo2WithoutAlert_FindsDivergence()
    {
        var now = DateTimeOffset.UtcNow;

        var records = new List<ClinicalRecordSnapshotProxy>
        {
            new(
                "vitals:1",
                "VitalSign",
                "Sinais vitais",
                "Medição em agora. SpO2 88%. FC 110 bpm.",
                now.AddHours(-1),
                null,
                null)
        };

        var findings = ChartAuditEngine.Evaluate(records, now);

        Assert.Contains(
            findings,
            f => f.Code == "DIV.LOW_SPO2_WITHOUT_ALERT");
    }

    [Fact]
    public void Evaluate_CompleteChart_ReturnsNoFindings()
    {
        var now = DateTimeOffset.UtcNow;

        var records = new List<ClinicalRecordSnapshotProxy>
        {
            new(
                "admission:1",
                "Admission",
                "UTI",
                "Internação status Active.",
                now.AddDays(-1),
                "Active",
                null),
            new(
                "note:1",
                "ClinicalNote",
                "Evolução",
                "Nota Medical",
                now.AddHours(-2),
                null,
                "Medical"),
            new(
                "vitals:1",
                "VitalSign",
                "Sinais vitais",
                "SpO2 97%. FC 80 bpm.",
                now.AddHours(-1),
                null,
                null),
            new(
                "exam:1",
                "Exam",
                "Hemograma",
                "Status: Resulted. Resultado: normal.",
                now.AddHours(-3),
                "Resulted",
                null)
        };

        var findings = ChartAuditEngine.Evaluate(records, now);

        Assert.Empty(findings);
    }
}
