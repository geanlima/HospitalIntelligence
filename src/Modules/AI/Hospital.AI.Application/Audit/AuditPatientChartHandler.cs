using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.Audit;

public sealed record AuditPatientChartQuery(
    Guid PatientId);

public sealed record AuditPatientChartResult(
    Guid PatientId,
    DateTimeOffset AuditedAtUtc,
    string OverallRisk,
    string Summary,
    int MissingDocumentationCount,
    int DivergenceCount,
    int FinancialGlosaRiskCount,
    IReadOnlyList<ChartAuditFinding> Findings);

public sealed class AuditPatientChartHandler
{
    private readonly IAiAccessPolicy _accessPolicy;
    private readonly IClinicalRecordSource _clinicalRecordSource;

    public AuditPatientChartHandler(
        IAiAccessPolicy accessPolicy,
        IClinicalRecordSource clinicalRecordSource)
    {
        _accessPolicy = accessPolicy;
        _clinicalRecordSource = clinicalRecordSource;
    }

    public async Task<Result<AuditPatientChartResult>> HandleAsync(
        AuditPatientChartQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PatientId == Guid.Empty)
        {
            return Result<AuditPatientChartResult>.Failure(
                new Error(
                    "AI.Audit.PatientIdRequired",
                    "PatientId é obrigatório para auditar o prontuário."));
        }

        var access =
            await _accessPolicy.EnsureCanAccessPatientAsync(
                query.PatientId,
                cancellationToken);

        if (access.IsFailure)
        {
            return Result<AuditPatientChartResult>.Failure(access.Error);
        }

        var records =
            await _clinicalRecordSource.GetByPatientIdAsync(
                query.PatientId,
                cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var proxies = records
            .Select(r => new ClinicalRecordSnapshotProxy(
                r.SourceId,
                r.RecordType,
                r.Title,
                r.Content,
                r.OccurredAtUtc,
                r.Status,
                r.SubType))
            .ToList();

        var findings = ChartAuditEngine.Evaluate(proxies, now);

        var missing = findings.Count(f =>
            f.Category == ChartAuditCategories.MissingDocumentation);

        var divergences = findings.Count(f =>
            f.Category == ChartAuditCategories.Divergence);

        var glosa = findings.Count(f =>
            f.Category == ChartAuditCategories.FinancialGlosaRisk);

        var overallRisk = ResolveOverallRisk(findings);

        var summary = findings.Count == 0
            ? "Nenhum achado de auditoria. Prontuário aparenta estar consistente com as regras atuais."
            : $"{findings.Count} achado(s): {missing} documentação ausente, " +
              $"{divergences} divergência(s), {glosa} risco(s) de glosa. " +
              $"Risco geral: {overallRisk}.";

        return Result<AuditPatientChartResult>.Success(
            new AuditPatientChartResult(
                query.PatientId,
                now,
                overallRisk,
                summary,
                missing,
                divergences,
                glosa,
                findings));
    }

    private static string ResolveOverallRisk(
        IReadOnlyList<ChartAuditFinding> findings)
    {
        if (findings.Count == 0)
        {
            return "None";
        }

        if (findings.Any(f => f.Severity == ChartAuditSeverities.Critical))
        {
            return ChartAuditSeverities.Critical;
        }

        if (findings.Any(f => f.Severity == ChartAuditSeverities.High))
        {
            return ChartAuditSeverities.High;
        }

        if (findings.Any(f => f.Severity == ChartAuditSeverities.Medium))
        {
            return ChartAuditSeverities.Medium;
        }

        return ChartAuditSeverities.Low;
    }
}
