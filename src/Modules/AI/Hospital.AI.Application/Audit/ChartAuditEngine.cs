namespace Hospital.AI.Application.Audit;

public static class ChartAuditCategories
{
    public const string MissingDocumentation = "MissingDocumentation";
    public const string Divergence = "Divergence";
    public const string FinancialGlosaRisk = "FinancialGlosaRisk";
}

public static class ChartAuditSeverities
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Critical = "Critical";
}

public sealed record ChartAuditFinding(
    string Code,
    string Category,
    string Severity,
    string Title,
    string Message,
    IReadOnlyList<string> RelatedSourceIds);

public static class ChartAuditEngine
{
    public static IReadOnlyList<ChartAuditFinding> Evaluate(
        IReadOnlyList<ClinicalRecordSnapshotProxy> records,
        DateTimeOffset nowUtc)
    {
        var findings = new List<ChartAuditFinding>();

        var admissions = records
            .Where(r => r.RecordType == "Admission")
            .ToList();

        var notes = records
            .Where(r => r.RecordType == "ClinicalNote")
            .ToList();

        var exams = records
            .Where(r => r.RecordType == "Exam")
            .ToList();

        var vitals = records
            .Where(r => r.RecordType == "VitalSign")
            .ToList();

        var alerts = records
            .Where(r => r.RecordType == "Alert")
            .ToList();

        var prescriptions = records
            .Where(r => r.RecordType == "Prescription")
            .ToList();

        var activeAdmissions = admissions
            .Where(a => EqualsIgnore(a.Status, "Active"))
            .ToList();

        EvaluateMissingDocumentation(
            findings,
            activeAdmissions,
            admissions,
            notes,
            exams,
            vitals);

        EvaluateDivergences(
            findings,
            exams,
            alerts,
            vitals,
            nowUtc);

        EvaluateGlosaRisk(
            findings,
            activeAdmissions,
            admissions,
            notes,
            exams,
            prescriptions);

        return findings
            .OrderByDescending(f => SeverityRank(f.Severity))
            .ThenBy(f => f.Code, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();
    }

    private static void EvaluateMissingDocumentation(
        List<ChartAuditFinding> findings,
        IReadOnlyList<ClinicalRecordSnapshotProxy> activeAdmissions,
        IReadOnlyList<ClinicalRecordSnapshotProxy> admissions,
        IReadOnlyList<ClinicalRecordSnapshotProxy> notes,
        IReadOnlyList<ClinicalRecordSnapshotProxy> exams,
        IReadOnlyList<ClinicalRecordSnapshotProxy> vitals)
    {
        if (activeAdmissions.Count > 0 && notes.Count == 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "DOC.NO_CLINICAL_NOTES",
                    ChartAuditCategories.MissingDocumentation,
                    ChartAuditSeverities.High,
                    "Internação sem notas clínicas",
                    "Há internação ativa sem nenhuma nota clínica registrada.",
                    activeAdmissions.Select(a => a.SourceId).ToList()));
        }

        if (activeAdmissions.Count > 0 &&
            !notes.Any(n =>
                EqualsIgnore(n.SubType, "Medical") ||
                EqualsIgnore(n.SubType, "Evolution")))
        {
            findings.Add(
                new ChartAuditFinding(
                    "DOC.NO_MEDICAL_EVOLUTION",
                    ChartAuditCategories.MissingDocumentation,
                    ChartAuditSeverities.High,
                    "Ausência de evolução médica",
                    "Internação ativa sem nota do tipo Medical ou Evolution.",
                    activeAdmissions.Select(a => a.SourceId).ToList()));
        }

        if (activeAdmissions.Count > 0 && vitals.Count == 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "DOC.NO_VITAL_SIGNS",
                    ChartAuditCategories.MissingDocumentation,
                    ChartAuditSeverities.Medium,
                    "Sem sinais vitais",
                    "Internação ativa sem registros de sinais vitais.",
                    activeAdmissions.Select(a => a.SourceId).ToList()));
        }

        var pendingExams = exams
            .Where(e =>
                EqualsIgnore(e.Status, "Requested") ||
                EqualsIgnore(e.Status, "InProgress"))
            .ToList();

        if (pendingExams.Count > 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "DOC.PENDING_EXAM_RESULT",
                    ChartAuditCategories.MissingDocumentation,
                    ChartAuditSeverities.Medium,
                    "Exames sem resultado",
                    $"{pendingExams.Count} exame(s) ainda sem resultado (Requested/InProgress).",
                    pendingExams.Select(e => e.SourceId).ToList()));
        }

        var discharged = admissions
            .Where(a => EqualsIgnore(a.Status, "Discharged"))
            .ToList();

        if (discharged.Count > 0 &&
            !notes.Any(n => EqualsIgnore(n.SubType, "Medical")))
        {
            findings.Add(
                new ChartAuditFinding(
                    "DOC.DISCHARGE_WITHOUT_MEDICAL_NOTE",
                    ChartAuditCategories.MissingDocumentation,
                    ChartAuditSeverities.High,
                    "Alta sem nota médica",
                    "Há alta registrada sem nota clínica do tipo Medical.",
                    discharged.Select(a => a.SourceId).ToList()));
        }
    }

    private static void EvaluateDivergences(
        List<ChartAuditFinding> findings,
        IReadOnlyList<ClinicalRecordSnapshotProxy> exams,
        IReadOnlyList<ClinicalRecordSnapshotProxy> alerts,
        IReadOnlyList<ClinicalRecordSnapshotProxy> vitals,
        DateTimeOffset nowUtc)
    {
        var resultedWithoutResult = exams
            .Where(e =>
                EqualsIgnore(e.Status, "Resulted") &&
                (string.IsNullOrWhiteSpace(e.Content) ||
                 e.Content.Contains(
                     "Resultado: pendente",
                     StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (resultedWithoutResult.Count > 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "DIV.EXAM_RESULTED_WITHOUT_TEXT",
                    ChartAuditCategories.Divergence,
                    ChartAuditSeverities.High,
                    "Exame concluído sem texto de resultado",
                    "Exame(s) com status Resulted, mas sem conteúdo de resultado.",
                    resultedWithoutResult.Select(e => e.SourceId).ToList()));
        }

        var activeCriticalAlerts = alerts
            .Where(a =>
                EqualsIgnore(a.Status, "Active") &&
                (EqualsIgnore(a.SubType, "Critical") ||
                 a.Content.Contains(
                     "Severidade: Critical",
                     StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (activeCriticalAlerts.Count > 0)
        {
            var recentVitals = vitals
                .Where(v => v.OccurredAtUtc >= nowUtc.AddHours(-12))
                .ToList();

            if (recentVitals.Count == 0)
            {
                findings.Add(
                    new ChartAuditFinding(
                        "DIV.CRITICAL_ALERT_WITHOUT_RECENT_VITALS",
                        ChartAuditCategories.Divergence,
                        ChartAuditSeverities.Critical,
                        "Alerta crítico sem sinais vitais recentes",
                        "Existe alerta crítico ativo sem medição de sinais vitais nas últimas 12h.",
                        activeCriticalAlerts.Select(a => a.SourceId).ToList()));
            }
        }

        var lowSpo2 = vitals
            .Where(v =>
                v.Content.Contains("SpO2", StringComparison.OrdinalIgnoreCase) &&
                TryParseSpo2(v.Content, out var spo2) &&
                spo2 < 92)
            .ToList();

        if (lowSpo2.Count > 0 &&
            !alerts.Any(a => EqualsIgnore(a.Status, "Active")))
        {
            findings.Add(
                new ChartAuditFinding(
                    "DIV.LOW_SPO2_WITHOUT_ALERT",
                    ChartAuditCategories.Divergence,
                    ChartAuditSeverities.High,
                    "Hipoxemia sem alerta ativo",
                    "Há registro de SpO2 < 92% sem alerta ativo correspondente.",
                    lowSpo2.Select(v => v.SourceId).ToList()));
        }
    }

    private static void EvaluateGlosaRisk(
        List<ChartAuditFinding> findings,
        IReadOnlyList<ClinicalRecordSnapshotProxy> activeAdmissions,
        IReadOnlyList<ClinicalRecordSnapshotProxy> admissions,
        IReadOnlyList<ClinicalRecordSnapshotProxy> notes,
        IReadOnlyList<ClinicalRecordSnapshotProxy> exams,
        IReadOnlyList<ClinicalRecordSnapshotProxy> prescriptions)
    {
        if (activeAdmissions.Count > 0 &&
            !notes.Any(n =>
                EqualsIgnore(n.SubType, "Medical") ||
                EqualsIgnore(n.SubType, "Evolution")))
        {
            findings.Add(
                new ChartAuditFinding(
                    "GLOSA.MISSING_DAILY_EVOLUTION",
                    ChartAuditCategories.FinancialGlosaRisk,
                    ChartAuditSeverities.High,
                    "Risco de glosa por evolução ausente",
                    "Falta evolução médica/diária — item frequente de glosa em auditoria de contas.",
                    activeAdmissions.Select(a => a.SourceId).ToList()));
        }

        var pendingExams = exams
            .Where(e =>
                EqualsIgnore(e.Status, "Requested") ||
                EqualsIgnore(e.Status, "InProgress"))
            .ToList();

        if (pendingExams.Count > 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "GLOSA.UNBILLED_PENDING_EXAM",
                    ChartAuditCategories.FinancialGlosaRisk,
                    ChartAuditSeverities.Medium,
                    "Risco de glosa por exame sem laudo",
                    "Exames sem resultado/laudo aumentam risco de glosa ou cobrança incompleta.",
                    pendingExams.Select(e => e.SourceId).ToList()));
        }

        var discharged = admissions
            .Where(a => EqualsIgnore(a.Status, "Discharged"))
            .ToList();

        if (discharged.Count > 0 &&
            !notes.Any(n => EqualsIgnore(n.SubType, "Medical")))
        {
            findings.Add(
                new ChartAuditFinding(
                    "GLOSA.DISCHARGE_WITHOUT_SUMMARY",
                    ChartAuditCategories.FinancialGlosaRisk,
                    ChartAuditSeverities.High,
                    "Risco de glosa na alta",
                    "Alta sem nota médica/resumo aumenta risco de glosa na conta hospitalar.",
                    discharged.Select(a => a.SourceId).ToList()));
        }

        var activePrescriptions = prescriptions
            .Where(p => EqualsIgnore(p.Status, "Active"))
            .ToList();

        if (activePrescriptions.Count > 0 && notes.Count == 0)
        {
            findings.Add(
                new ChartAuditFinding(
                    "GLOSA.PRESCRIPTION_WITHOUT_JUSTIFICATION",
                    ChartAuditCategories.FinancialGlosaRisk,
                    ChartAuditSeverities.Medium,
                    "Prescrição sem justificativa documental",
                    "Prescrições ativas sem notas clínicas de suporte elevam risco de glosa.",
                    activePrescriptions.Select(p => p.SourceId).ToList()));
        }
    }

    private static bool TryParseSpo2(string content, out decimal spo2)
    {
        spo2 = 0;
        const string marker = "SpO2 ";
        var index = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
        {
            return false;
        }

        var start = index + marker.Length;
        var end = start;

        while (end < content.Length &&
               (char.IsDigit(content[end]) || content[end] == '.' || content[end] == ','))
        {
            end++;
        }

        if (end == start)
        {
            return false;
        }

        var raw = content[start..end].Replace(',', '.');
        return decimal.TryParse(
            raw,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture,
            out spo2);
    }

    private static bool EqualsIgnore(string? left, string right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }

    private static int SeverityRank(string severity)
    {
        return severity switch
        {
            ChartAuditSeverities.Critical => 4,
            ChartAuditSeverities.High => 3,
            ChartAuditSeverities.Medium => 2,
            ChartAuditSeverities.Low => 1,
            _ => 0
        };
    }
}

/// <summary>
/// Proxy tipado para o motor de regras (evita acoplar Application ao Domain clínico).
/// </summary>
public sealed record ClinicalRecordSnapshotProxy(
    string SourceId,
    string RecordType,
    string Title,
    string Content,
    DateTimeOffset OccurredAtUtc,
    string? Status,
    string? SubType);
