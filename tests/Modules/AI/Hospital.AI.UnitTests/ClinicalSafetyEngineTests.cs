using Hospital.AI.Application.ClinicalSafety;

namespace Hospital.AI.UnitTests;

public class ClinicalSafetyEngineTests
{
    [Fact]
    public void Evaluate_ActiveAdmissionWithRisks_BlocksDischarge()
    {
        var now = DateTimeOffset.UtcNow;

        var assessment = ClinicalSafetyEngine.Evaluate(
            [
                new ClinicalSafetyRecord(
                    "admission:1",
                    "Admission",
                    "UTI",
                    "Internação status Active.",
                    now.AddDays(-1),
                    "Active",
                    null),
                new ClinicalSafetyRecord(
                    "alert:1",
                    "Alert",
                    "SpO2",
                    "Severidade: Critical. Status: Active.",
                    now.AddHours(-1),
                    "Active",
                    "Critical"),
                new ClinicalSafetyRecord(
                    "vitals:1",
                    "VitalSign",
                    "Sinais",
                    "SpO2 88%. FC 130 bpm. PA 85/50 mmHg. Temp 39.2 °C. FR 28 rpm.",
                    now.AddHours(-1),
                    null,
                    null)
            ],
            now);

        Assert.False(assessment.DischargeReady);
        Assert.True(assessment.DischargeBlockerCount > 0);
        Assert.True(assessment.DeteriorationScore >= 7);
        Assert.Equal(ClinicalSafetySeverities.Critical, assessment.DeteriorationBand);
        Assert.Contains(
            assessment.Findings,
            f => f.Code == "DISCHARGE.OPEN_CRITICAL_ALERTS");
        Assert.Contains(
            assessment.Findings,
            f => f.Code == "TRIAGE.URGENCY_BAND");
    }

    [Fact]
    public void Evaluate_DuplicateActivePrescriptions_FindsMedRecIssue()
    {
        var now = DateTimeOffset.UtcNow;

        var assessment = ClinicalSafetyEngine.Evaluate(
            [
                new ClinicalSafetyRecord(
                    "rx:1",
                    "Prescription",
                    "Prescrição",
                    "Prescrição: Dipirona 500mg. Status: Active. Data: agora.",
                    now,
                    "Active",
                    null),
                new ClinicalSafetyRecord(
                    "rx:2",
                    "Prescription",
                    "Prescrição",
                    "Prescrição: Dipirona 500mg. Status: Active. Data: agora.",
                    now,
                    "Active",
                    null)
            ],
            now);

        Assert.True(assessment.MedicationIssueCount > 0);
        Assert.Contains(
            assessment.Findings,
            f => f.Code == "MEDREC.DUPLICATE_ACTIVE");
    }
}
