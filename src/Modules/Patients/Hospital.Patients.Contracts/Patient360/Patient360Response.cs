using Hospital.Patients.Contracts.Patient360;

public sealed record Patient360Response(
    Guid PatientId,
    string Name,
    DateOnly BirthDate,
    string Gender,
    string? SourceSystem,
    string? ExternalId,
    IReadOnlyCollection<AdmissionSummaryResponse> Admissions,
    IReadOnlyCollection<ExamSummaryResponse> Exams,
    IReadOnlyCollection<PrescriptionSummaryResponse> Prescriptions,
    IReadOnlyCollection<VitalSignSummaryResponse> VitalSigns,
    IReadOnlyCollection<ClinicalNoteSummaryResponse> ClinicalNotes,
    IReadOnlyCollection<PatientAlertResponse> Alerts,
    IReadOnlyCollection<PatientTimelineItemResponse> Timeline);