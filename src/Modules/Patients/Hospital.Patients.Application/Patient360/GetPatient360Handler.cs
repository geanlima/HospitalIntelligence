using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Contracts.Patient360;
using Hospital.Prescriptions.Application.Abstractions;
using Hospital.SharedKernel.Application;
using Hospital.Timeline.Application.Timeline.Abstractions;
using Hospital.VitalSigns.Application.VitalSigns.Abstractions;

namespace Hospital.Patients.Application.Patient360;

public sealed class GetPatient360Handler
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IExamRepository _examRepository;
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IVitalSignRepository _vitalSignRepository;
    private readonly IClinicalNoteRepository _clinicalNoteRepository;
    private readonly IPatientAlertRepository _patientAlertRepository;
    private readonly ITimelineRepository _timelineRepository;

    public GetPatient360Handler(
        IPatientRepository patientRepository,
        IAdmissionRepository admissionRepository,
        IExamRepository examRepository,
        IPrescriptionRepository prescriptionRepository,
        IVitalSignRepository vitalSignRepository,
        IClinicalNoteRepository clinicalNoteRepository,
        IPatientAlertRepository patientAlertRepository,
        ITimelineRepository timelineRepository)
    {
        _patientRepository = patientRepository;
        _admissionRepository = admissionRepository;
        _examRepository = examRepository;
        _prescriptionRepository = prescriptionRepository;
        _vitalSignRepository = vitalSignRepository;
        _clinicalNoteRepository = clinicalNoteRepository;
        _patientAlertRepository = patientAlertRepository;
        _timelineRepository = timelineRepository;
    }

    public async Task<Result<Patient360Response>> HandleAsync(
        GetPatient360Query query,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(
            query.PatientId,
            cancellationToken);

        if (patient is null)
        {
            return Result<Patient360Response>.Failure(
                new Error(
                    "Patient.NotFound",
                    "Patient was not found."));
        }

        var patientId = patient.Id.Value;

        var admissions =
            await _admissionRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var exams =
            await _examRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var prescriptions =
            await _prescriptionRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var vitalSigns =
            await _vitalSignRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var clinicalNotes =
            await _clinicalNoteRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var alerts =
            await _patientAlertRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var timelineItems =
            await _timelineRepository.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        var admissionResponses =
            admissions
                .Select(admission =>
                    new AdmissionSummaryResponse(
                        admission.Id.Value,
                        admission.AdmissionDate,
                        admission.DischargeDate,
                        admission.Unit,
                        admission.Bed,
                        admission.Status.ToString()))
                .ToList();

        var examResponses =
            exams
                .Select(exam =>
                    new ExamSummaryResponse(
                        exam.Id.Value,
                        exam.Name,
                        exam.RequestedAtUtc,
                        exam.ResultedAtUtc,
                        exam.Status.ToString()))
                .ToList();

        var prescriptionResponses =
            prescriptions
                .Select(prescription =>
                    new PrescriptionSummaryResponse(
                        prescription.Id.Value,
                        prescription.Description,
                        prescription.PrescribedAtUtc,
                        prescription.Status.ToString()))
                .ToList();

        var vitalSignResponses =
            vitalSigns
                .Select(vitalSign =>
                    new VitalSignSummaryResponse(
                        vitalSign.Id.Value,
                        vitalSign.MeasuredAtUtc,
                        vitalSign.Temperature,
                        vitalSign.HeartRate,
                        vitalSign.RespiratoryRate,
                        vitalSign.SystolicBloodPressure,
                        vitalSign.DiastolicBloodPressure,
                        vitalSign.OxygenSaturation))
                .ToList();

        var clinicalNoteResponses =
            clinicalNotes
                .Select(note =>
                    new ClinicalNoteSummaryResponse(
                        note.Id.Value,
                        note.CreatedAtUtc,
                        note.Professional,
                        note.NoteType.ToString(),
                        note.Content))
                .ToList();

        var alertResponses =
            alerts
                .Select(alert =>
                    new PatientAlertResponse(
                        alert.Id.Value,
                        alert.Type,
                        alert.Severity.ToString(),
                        alert.Description,
                        alert.CreatedAtUtc))
                .ToList();

        var timelineResponses =
            timelineItems
                .Select(item =>
                    new PatientTimelineItemResponse(
                        item.Id.Value,
                        item.OccurredAtUtc,
                        item.Type,
                        item.Title,
                        item.Description))
                .ToList();

        var response =
            new Patient360Response(
                patientId,
                patient.Name,
                patient.BirthDate,
                patient.Gender.ToString(),
                patient.ExternalIdentifier?.SourceSystem,
                patient.ExternalIdentifier?.ExternalId,
                admissionResponses,
                examResponses,
                prescriptionResponses,
                vitalSignResponses,
                clinicalNoteResponses,
                alertResponses,
                timelineResponses);

        return Result<Patient360Response>.Success(response);
    }
}