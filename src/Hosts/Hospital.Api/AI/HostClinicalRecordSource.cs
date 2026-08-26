using System.Globalization;
using System.Text;
using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.AI.Application.Abstractions;
using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.Prescriptions.Application.Abstractions;
using Hospital.VitalSigns.Application.VitalSigns.Abstractions;

namespace Hospital.Api.AI;

/// <summary>
/// Composição no Host: lê módulos clínicos e projeta snapshots para o módulo AI.
/// </summary>
public sealed class HostClinicalRecordSource : IClinicalRecordSource
{
    private readonly IPatientRepository _patients;
    private readonly IAdmissionRepository _admissions;
    private readonly IExamRepository _exams;
    private readonly IPrescriptionRepository _prescriptions;
    private readonly IVitalSignRepository _vitalSigns;
    private readonly IClinicalNoteRepository _clinicalNotes;
    private readonly IPatientAlertRepository _alerts;

    public HostClinicalRecordSource(
        IPatientRepository patients,
        IAdmissionRepository admissions,
        IExamRepository exams,
        IPrescriptionRepository prescriptions,
        IVitalSignRepository vitalSigns,
        IClinicalNoteRepository clinicalNotes,
        IPatientAlertRepository alerts)
    {
        _patients = patients;
        _admissions = admissions;
        _exams = exams;
        _prescriptions = prescriptions;
        _vitalSigns = vitalSigns;
        _clinicalNotes = clinicalNotes;
        _alerts = alerts;
    }

    public async Task<bool> PatientExistsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetByIdAsync(
            new PatientId(patientId),
            cancellationToken);

        return patient is not null;
    }

    public async Task<IReadOnlyList<ClinicalRecordSnapshot>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var snapshots = new List<ClinicalRecordSnapshot>();

        var patient = await _patients.GetByIdAsync(
            new PatientId(patientId),
            cancellationToken);

        if (patient is null)
        {
            return snapshots;
        }

        snapshots.Add(
            new ClinicalRecordSnapshot(
                $"patient:{patient.Id.Value}",
                "Patient",
                $"Paciente - {patient.Name}",
                $"Nome: {patient.Name}. Nascimento: {patient.BirthDate:yyyy-MM-dd}. Sexo: {patient.Gender}.",
                patientId,
                patient.UpdatedAtUtc));

        var admissions =
            await _admissions.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var admission in admissions)
        {
            var content =
                $"Internação status {admission.Status}. " +
                $"Data: {admission.AdmissionDate:u}. " +
                $"Unidade: {admission.Unit ?? "-"}. Leito: {admission.Bed ?? "-"}. " +
                $"Alta: {(admission.DischargeDate?.ToString("u") ?? "não")}.";

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"admission:{admission.Id.Value}",
                    "Admission",
                    $"Internação - {admission.Unit ?? "sem unidade"}",
                    content,
                    patientId,
                    admission.AdmissionDate,
                    admission.Status.ToString()));
        }

        var exams =
            await _exams.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var exam in exams)
        {
            var content =
                $"Exame {exam.Name}. Status: {exam.Status}. " +
                $"Solicitado: {exam.RequestedAtUtc:u}. " +
                $"Resultado: {exam.Result ?? "pendente"}.";

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"exam:{exam.Id.Value}",
                    "Exam",
                    $"Exame - {exam.Name}",
                    content,
                    patientId,
                    exam.RequestedAtUtc,
                    exam.Status.ToString()));
        }

        var prescriptions =
            await _prescriptions.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var prescription in prescriptions)
        {
            var content =
                $"Prescrição: {prescription.Description}. " +
                $"Status: {prescription.Status}. " +
                $"Data: {prescription.PrescribedAtUtc:u}.";

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"prescription:{prescription.Id.Value}",
                    "Prescription",
                    "Prescrição",
                    content,
                    patientId,
                    prescription.PrescribedAtUtc,
                    prescription.Status.ToString()));
        }

        var vitalSigns =
            await _vitalSigns.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var vital in vitalSigns)
        {
            var content = BuildVitalSignContent(vital);

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"vitals:{vital.Id.Value}",
                    "VitalSign",
                    "Sinais vitais",
                    content,
                    patientId,
                    vital.MeasuredAtUtc));
        }

        var notes =
            await _clinicalNotes.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var note in notes)
        {
            var content =
                $"Nota ({note.NoteType}) por {note.Professional} em {note.CreatedAtUtc:u}. " +
                note.Content;

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"note:{note.Id.Value}",
                    "ClinicalNote",
                    $"Nota clínica - {note.NoteType}",
                    content,
                    patientId,
                    note.CreatedAtUtc,
                    Status: null,
                    SubType: note.NoteType.ToString()));
        }

        var alerts =
            await _alerts.GetByPatientIdAsync(patientId, cancellationToken);

        foreach (var alert in alerts)
        {
            var content =
                $"Alerta {alert.Type}. Severidade: {alert.Severity}. " +
                $"Status: {alert.Status}. {alert.Description}";

            snapshots.Add(
                new ClinicalRecordSnapshot(
                    $"alert:{alert.Id.Value}",
                    "Alert",
                    $"Alerta - {alert.Type}",
                    content,
                    patientId,
                    alert.CreatedAtUtc,
                    alert.Status.ToString(),
                    alert.Severity.ToString()));
        }

        return snapshots;
    }

    private static string BuildVitalSignContent(
        Hospital.VitalSigns.Domain.VitalSigns.VitalSign vital)
    {
        var parts = new StringBuilder();
        parts.Append(CultureInfo.InvariantCulture, $"Medição em {vital.MeasuredAtUtc:u}. ");

        if (vital.OxygenSaturation is not null)
        {
            parts.Append(CultureInfo.InvariantCulture, $"SpO2 {vital.OxygenSaturation}%. ");
        }

        if (vital.HeartRate is not null)
        {
            parts.Append(CultureInfo.InvariantCulture, $"FC {vital.HeartRate} bpm. ");
        }

        if (vital.SystolicBloodPressure is not null ||
            vital.DiastolicBloodPressure is not null)
        {
            parts.Append(
                CultureInfo.InvariantCulture,
                $"PA {vital.SystolicBloodPressure}/{vital.DiastolicBloodPressure} mmHg. ");
        }

        if (vital.Temperature is not null)
        {
            parts.Append(CultureInfo.InvariantCulture, $"Temp {vital.Temperature} °C. ");
        }

        if (vital.RespiratoryRate is not null)
        {
            parts.Append(CultureInfo.InvariantCulture, $"FR {vital.RespiratoryRate} rpm.");
        }

        return parts.ToString().Trim();
    }
}
