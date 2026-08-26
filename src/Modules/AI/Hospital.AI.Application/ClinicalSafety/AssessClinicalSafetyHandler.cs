using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.ClinicalSafety;

public sealed record AssessClinicalSafetyQuery(
    Guid PatientId);

public sealed record AssessClinicalSafetyResult(
    Guid PatientId,
    DateTimeOffset AssessedAtUtc,
    string OverallRisk,
    string Summary,
    bool DischargeReady,
    int DischargeBlockerCount,
    int DeteriorationScore,
    string DeteriorationBand,
    string TriageRecommendation,
    int MedicationIssueCount,
    IReadOnlyList<ClinicalSafetyFinding> Findings);

public sealed class AssessClinicalSafetyHandler
{
    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IClinicalRecordSource _clinicalRecordSource;

    public AssessClinicalSafetyHandler(
        IAiAccessPolicy accessPolicy,
        IClinicalRecordSource clinicalRecordSource)
    {
        _accessPolicy = accessPolicy;
        _clinicalRecordSource = clinicalRecordSource;
    }

    public async Task<Result<AssessClinicalSafetyResult>> HandleAsync(
        AssessClinicalSafetyQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PatientId == Guid.Empty)
        {
            return Result<AssessClinicalSafetyResult>.Failure(
                new Error(
                    "AI.ClinicalSafety.PatientIdRequired",
                    "PatientId é obrigatório para avaliação de segurança clínica."));
        }

        var access =
            await _accessPolicy.EnsureCanAccessPatientAsync(
                query.PatientId,
                cancellationToken);

        if (access.IsFailure)
        {
            return Result<AssessClinicalSafetyResult>.Failure(access.Error);
        }

        var records =
            await _clinicalRecordSource.GetByPatientIdAsync(
                query.PatientId,
                cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var proxies = records
            .Select(r => new ClinicalSafetyRecord(
                r.SourceId,
                r.RecordType,
                r.Title,
                r.Content,
                r.OccurredAtUtc,
                r.Status,
                r.SubType))
            .ToList();

        var assessment = ClinicalSafetyEngine.Evaluate(proxies, now);
        var overallRisk = ResolveOverallRisk(assessment);

        var summary =
            $"Alta segura: {(assessment.DischargeReady ? "pronta" : "bloqueada")} " +
            $"({assessment.DischargeBlockerCount} bloqueio(s)). " +
            $"Deterioração NEWS2-lite={assessment.DeteriorationScore} ({assessment.DeteriorationBand}). " +
            $"Reconciliação: {assessment.MedicationIssueCount} issue(s). " +
            assessment.TriageRecommendation;

        return Result<AssessClinicalSafetyResult>.Success(
            new AssessClinicalSafetyResult(
                query.PatientId,
                now,
                overallRisk,
                summary,
                assessment.DischargeReady,
                assessment.DischargeBlockerCount,
                assessment.DeteriorationScore,
                assessment.DeteriorationBand,
                assessment.TriageRecommendation,
                assessment.MedicationIssueCount,
                assessment.Findings));
    }

    private static string ResolveOverallRisk(
        ClinicalSafetyAssessment assessment)
    {
        if (assessment.Findings.Any(f =>
                f.Severity == ClinicalSafetySeverities.Critical))
        {
            return ClinicalSafetySeverities.Critical;
        }

        if (assessment.Findings.Any(f =>
                f.Severity == ClinicalSafetySeverities.High) ||
            assessment.DeteriorationBand is ClinicalSafetySeverities.High
                or ClinicalSafetySeverities.Critical)
        {
            return ClinicalSafetySeverities.High;
        }

        if (assessment.Findings.Any(f =>
                f.Severity == ClinicalSafetySeverities.Medium) ||
            assessment.DeteriorationBand == ClinicalSafetySeverities.Medium)
        {
            return ClinicalSafetySeverities.Medium;
        }

        if (assessment.Findings.Count == 0 &&
            assessment.DeteriorationBand == "None")
        {
            return "None";
        }

        return ClinicalSafetySeverities.Low;
    }
}
