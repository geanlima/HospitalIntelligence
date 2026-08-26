namespace Hospital.AI.Application.ClinicalSafety;

public static class ClinicalSafetyCategories
{
    public const string DischargeSafety = "DischargeSafety";
    public const string Deterioration = "Deterioration";
    public const string MedicationReconciliation = "MedicationReconciliation";
    public const string TriageAssist = "TriageAssist";
}

public static class ClinicalSafetySeverities
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Critical = "Critical";
}

public sealed record ClinicalSafetyFinding(
    string Code,
    string Category,
    string Severity,
    string Title,
    string Message,
    IReadOnlyList<string> RelatedSourceIds);

public sealed record ClinicalSafetyAssessment(
    IReadOnlyList<ClinicalSafetyFinding> Findings,
    int DeteriorationScore,
    string DeteriorationBand,
    string TriageRecommendation,
    bool DischargeReady,
    int DischargeBlockerCount,
    int MedicationIssueCount);

public static class ClinicalSafetyEngine
{
    public static ClinicalSafetyAssessment Evaluate(
        IReadOnlyList<ClinicalSafetyRecord> records,
        DateTimeOffset nowUtc)
    {
        var findings = new List<ClinicalSafetyFinding>();

        var admissions = records.Where(r => r.RecordType == "Admission").ToList();
        var notes = records.Where(r => r.RecordType == "ClinicalNote").ToList();
        var exams = records.Where(r => r.RecordType == "Exam").ToList();
        var vitals = records.Where(r => r.RecordType == "VitalSign").ToList();
        var alerts = records.Where(r => r.RecordType == "Alert").ToList();
        var prescriptions = records.Where(r => r.RecordType == "Prescription").ToList();

        var activeAdmissions = admissions
            .Where(a => EqualsIgnore(a.Status, "Active"))
            .ToList();

        EvaluateDischargeSafety(
            findings,
            activeAdmissions,
            notes,
            exams,
            vitals,
            alerts,
            prescriptions,
            nowUtc);

        var (score, band) = EvaluateDeterioration(findings, vitals);

        EvaluateMedicationReconciliation(findings, prescriptions, notes);

        var triage = BuildTriageRecommendation(score, band, findings);
        findings.Add(
            new ClinicalSafetyFinding(
                "TRIAGE.URGENCY_BAND",
                ClinicalSafetyCategories.TriageAssist,
                band == "None" ? ClinicalSafetySeverities.Low : band,
                "Sugestão de prioridade (copiloto)",
                triage,
                vitals.Select(v => v.SourceId).Take(3).ToList()));

        var dischargeBlockers = findings.Count(f =>
            f.Category == ClinicalSafetyCategories.DischargeSafety &&
            f.Severity is ClinicalSafetySeverities.High or ClinicalSafetySeverities.Critical);

        var medIssues = findings.Count(f =>
            f.Category == ClinicalSafetyCategories.MedicationReconciliation);

        var ordered = findings
            .OrderByDescending(f => SeverityRank(f.Severity))
            .ThenBy(f => f.Code, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();

        return new ClinicalSafetyAssessment(
            ordered,
            score,
            band,
            triage,
            dischargeBlockers == 0 && activeAdmissions.Count > 0,
            dischargeBlockers,
            medIssues);
    }

    private static void EvaluateDischargeSafety(
        List<ClinicalSafetyFinding> findings,
        IReadOnlyList<ClinicalSafetyRecord> activeAdmissions,
        IReadOnlyList<ClinicalSafetyRecord> notes,
        IReadOnlyList<ClinicalSafetyRecord> exams,
        IReadOnlyList<ClinicalSafetyRecord> vitals,
        IReadOnlyList<ClinicalSafetyRecord> alerts,
        IReadOnlyList<ClinicalSafetyRecord> prescriptions,
        DateTimeOffset nowUtc)
    {
        if (activeAdmissions.Count == 0)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.NO_ACTIVE_ADMISSION",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.Low,
                    "Sem internação ativa",
                    "Não há internação ativa para checklist de alta segura.",
                    []));
            return;
        }

        var related = activeAdmissions.Select(a => a.SourceId).ToList();

        if (!notes.Any(n =>
                EqualsIgnore(n.SubType, "Medical") ||
                EqualsIgnore(n.SubType, "Evolution")))
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.MISSING_EVOLUTION",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.High,
                    "Alta bloqueada: sem evolução médica",
                    "Checklist de alta segura exige nota Medical ou Evolution recente.",
                    related));
        }

        var openCritical = alerts
            .Where(a =>
                EqualsIgnore(a.Status, "Active") &&
                (EqualsIgnore(a.SubType, "Critical") ||
                 EqualsIgnore(a.SubType, "High")))
            .ToList();

        if (openCritical.Count > 0)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.OPEN_CRITICAL_ALERTS",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.Critical,
                    "Alta bloqueada: alertas abertos",
                    $"{openCritical.Count} alerta(s) Critical/High ativos impedem alta segura.",
                    openCritical.Select(a => a.SourceId).ToList()));
        }

        var pendingExams = exams
            .Where(e =>
                EqualsIgnore(e.Status, "Requested") ||
                EqualsIgnore(e.Status, "InProgress"))
            .ToList();

        if (pendingExams.Count > 0)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.PENDING_EXAMS",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.High,
                    "Alta bloqueada: exames pendentes",
                    $"{pendingExams.Count} exame(s) sem resultado.",
                    pendingExams.Select(e => e.SourceId).ToList()));
        }

        var latestVital = vitals
            .OrderByDescending(v => v.OccurredAtUtc)
            .FirstOrDefault();

        if (latestVital is null ||
            latestVital.OccurredAtUtc < nowUtc.AddHours(-24))
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.STALE_OR_MISSING_VITALS",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.High,
                    "Alta bloqueada: sinais vitais desatualizados",
                    "Não há sinais vitais nas últimas 24h.",
                    latestVital is null ? related : [latestVital.SourceId]));
        }

        var activeRx = prescriptions
            .Where(p => EqualsIgnore(p.Status, "Active"))
            .ToList();

        var hasDischargePlanNote = notes.Any(n =>
            n.Content.Contains("alta", StringComparison.OrdinalIgnoreCase) ||
            n.Content.Contains("discharge", StringComparison.OrdinalIgnoreCase));

        if (activeRx.Count > 0 && !hasDischargePlanNote)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DISCHARGE.NO_MEDICATION_PLAN",
                    ClinicalSafetyCategories.DischargeSafety,
                    ClinicalSafetySeverities.Medium,
                    "Plano medicamentoso de alta ausente",
                    "Há prescrições ativas sem nota mencionando plano de alta/medicação.",
                    activeRx.Select(p => p.SourceId).ToList()));
        }
    }

    private static (int Score, string Band) EvaluateDeterioration(
        List<ClinicalSafetyFinding> findings,
        IReadOnlyList<ClinicalSafetyRecord> vitals)
    {
        var latest = vitals
            .OrderByDescending(v => v.OccurredAtUtc)
            .FirstOrDefault();

        if (latest is null)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "DETER.NO_VITALS",
                    ClinicalSafetyCategories.Deterioration,
                    ClinicalSafetySeverities.Medium,
                    "Sem dados para escore de deterioração",
                    "Não há sinais vitais para calcular NEWS2-lite.",
                    []));
            return (0, "None");
        }

        var score = 0;
        var parts = new List<string>();

        if (TryParseDecimal(latest.Content, "FR ", out var rr) ||
            TryParseDecimal(latest.Content, "FR ", " rpm", out rr))
        {
            var points = ScoreRespiratoryRate(rr);
            score += points;
            parts.Add($"FR {rr}→{points}");
        }

        if (TryParseDecimal(latest.Content, "SpO2 ", out var spo2))
        {
            var points = ScoreSpo2(spo2);
            score += points;
            parts.Add($"SpO2 {spo2}→{points}");
        }

        if (TryParseBloodPressure(latest.Content, out var sbp, out _))
        {
            var points = ScoreSystolic(sbp);
            score += points;
            parts.Add($"PAS {sbp}→{points}");
        }

        if (TryParseDecimal(latest.Content, "FC ", out var hr))
        {
            var points = ScoreHeartRate(hr);
            score += points;
            parts.Add($"FC {hr}→{points}");
        }

        if (TryParseDecimal(latest.Content, "Temp ", out var temp))
        {
            var points = ScoreTemperature(temp);
            score += points;
            parts.Add($"Temp {temp}→{points}");
        }

        var band = score switch
        {
            >= 7 => ClinicalSafetySeverities.Critical,
            >= 5 => ClinicalSafetySeverities.High,
            >= 3 => ClinicalSafetySeverities.Medium,
            _ => ClinicalSafetySeverities.Low
        };

        findings.Add(
            new ClinicalSafetyFinding(
                "DETER.NEWS2_LITE",
                ClinicalSafetyCategories.Deterioration,
                band,
                $"Escore de deterioração NEWS2-lite: {score}",
                parts.Count == 0
                    ? "Vitals presentes, mas parâmetros não puderam ser parseados."
                    : $"Componentes: {string.Join("; ", parts)}.",
                [latest.SourceId]));

        return (score, band);
    }

    private static void EvaluateMedicationReconciliation(
        List<ClinicalSafetyFinding> findings,
        IReadOnlyList<ClinicalSafetyRecord> prescriptions,
        IReadOnlyList<ClinicalSafetyRecord> notes)
    {
        var active = prescriptions
            .Where(p => EqualsIgnore(p.Status, "Active"))
            .ToList();

        var normalized = active
            .Select(p => (
                Record: p,
                Key: NormalizeDrugKey(p.Content)))
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .ToList();

        var duplicates = normalized
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in duplicates)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "MEDREC.DUPLICATE_ACTIVE",
                    ClinicalSafetyCategories.MedicationReconciliation,
                    ClinicalSafetySeverities.High,
                    "Possível duplicidade medicamentosa",
                    $"Mais de uma prescrição ativa semelhante: '{group.Key}'.",
                    group.Select(g => g.Record.SourceId).ToList()));
        }

        var suspended = prescriptions
            .Where(p => EqualsIgnore(p.Status, "Suspended"))
            .ToList();

        foreach (var activeRx in active)
        {
            var key = NormalizeDrugKey(activeRx.Content);
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var clash = suspended.FirstOrDefault(s =>
                string.Equals(
                    NormalizeDrugKey(s.Content),
                    key,
                    StringComparison.OrdinalIgnoreCase));

            if (clash is not null)
            {
                findings.Add(
                    new ClinicalSafetyFinding(
                        "MEDREC.ACTIVE_AND_SUSPENDED",
                        ClinicalSafetyCategories.MedicationReconciliation,
                        ClinicalSafetySeverities.Medium,
                        "Conflito ativo/suspenso",
                        $"Prescrição ativa e suspensa com descrição semelhante ('{key}').",
                        [activeRx.SourceId, clash.SourceId]));
            }
        }

        if (active.Count > 0 && notes.Count == 0)
        {
            findings.Add(
                new ClinicalSafetyFinding(
                    "MEDREC.NO_CLINICAL_CONTEXT",
                    ClinicalSafetyCategories.MedicationReconciliation,
                    ClinicalSafetySeverities.Medium,
                    "Prescrições sem contexto clínico",
                    "Há medicamentos ativos sem notas clínicas de suporte à reconciliação.",
                    active.Select(p => p.SourceId).ToList()));
        }
    }

    private static string BuildTriageRecommendation(
        int score,
        string band,
        IReadOnlyList<ClinicalSafetyFinding> findings)
    {
        if (band is ClinicalSafetySeverities.Critical or ClinicalSafetySeverities.High ||
            findings.Any(f => f.Code == "DISCHARGE.OPEN_CRITICAL_ALERTS"))
        {
            return $"Prioridade elevada (NEWS2-lite={score}). Avaliar em caráter urgente / reavaliação imediata.";
        }

        if (band == ClinicalSafetySeverities.Medium)
        {
            return $"Prioridade intermediária (NEWS2-lite={score}). Monitorar e reavaliar em curto intervalo.";
        }

        return $"Prioridade padrão (NEWS2-lite={score}). Fluxo habitual de acompanhamento.";
    }

    private static string NormalizeDrugKey(string content)
    {
        var text = content;
        const string prefix = "Prescrição:";
        var idx = text.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            text = text[(idx + prefix.Length)..];
        }

        var statusIdx = text.IndexOf(". Status:", StringComparison.OrdinalIgnoreCase);
        if (statusIdx >= 0)
        {
            text = text[..statusIdx];
        }

        return string.Join(
            ' ',
            text.Trim()
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(4));
    }

    private static int ScoreRespiratoryRate(decimal rr) => rr switch
    {
        <= 8 => 3,
        <= 11 => 1,
        <= 20 => 0,
        <= 24 => 2,
        _ => 3
    };

    private static int ScoreSpo2(decimal spo2) => spo2 switch
    {
        <= 91 => 3,
        <= 93 => 2,
        <= 95 => 1,
        _ => 0
    };

    private static int ScoreSystolic(decimal sbp) => sbp switch
    {
        <= 90 => 3,
        <= 100 => 2,
        <= 110 => 1,
        < 220 => 0,
        _ => 3
    };

    private static int ScoreHeartRate(decimal hr) => hr switch
    {
        <= 40 => 3,
        <= 50 => 1,
        <= 90 => 0,
        <= 110 => 1,
        <= 130 => 2,
        _ => 3
    };

    private static int ScoreTemperature(decimal temp) => temp switch
    {
        <= 35.0m => 3,
        <= 36.0m => 1,
        <= 38.0m => 0,
        <= 39.0m => 1,
        _ => 2
    };

    private static bool TryParseDecimal(string content, string marker, out decimal value)
    {
        value = 0;
        var index = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return false;
        }

        var start = index + marker.Length;
        var end = start;
        while (end < content.Length &&
               (char.IsDigit(content[end]) || content[end] is '.' or ','))
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
            out value);
    }

    private static bool TryParseDecimal(
        string content,
        string marker,
        string _,
        out decimal value) =>
        TryParseDecimal(content, marker, out value);

    private static bool TryParseBloodPressure(
        string content,
        out decimal systolic,
        out decimal diastolic)
    {
        systolic = 0;
        diastolic = 0;
        const string marker = "PA ";
        var index = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return false;
        }

        var start = index + marker.Length;
        var slash = content.IndexOf('/', start);
        if (slash < 0)
        {
            return false;
        }

        var sysRaw = content[start..slash].Trim();
        var end = slash + 1;
        while (end < content.Length &&
               (char.IsDigit(content[end]) || content[end] is '.' or ','))
        {
            end++;
        }

        var diaRaw = content[(slash + 1)..end].Replace(',', '.');
        sysRaw = sysRaw.Replace(',', '.');

        return decimal.TryParse(
                   sysRaw,
                   System.Globalization.NumberStyles.Number,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out systolic) &&
               decimal.TryParse(
                   diaRaw,
                   System.Globalization.NumberStyles.Number,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out diastolic);
    }

    private static bool EqualsIgnore(string? left, string right) =>
        string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static int SeverityRank(string severity) => severity switch
    {
        ClinicalSafetySeverities.Critical => 4,
        ClinicalSafetySeverities.High => 3,
        ClinicalSafetySeverities.Medium => 2,
        ClinicalSafetySeverities.Low => 1,
        _ => 0
    };
}

public sealed record ClinicalSafetyRecord(
    string SourceId,
    string RecordType,
    string Title,
    string Content,
    DateTimeOffset OccurredAtUtc,
    string? Status,
    string? SubType);
