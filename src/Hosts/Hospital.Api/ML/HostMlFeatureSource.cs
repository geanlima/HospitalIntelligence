using System.Globalization;
using Hospital.Admissions.Application.Admissions.Abstractions;
using Hospital.Alerts.Application.Alerts.Abstractions;
using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.Exams.Application.Exams.Abstractions;
using Hospital.ML.Application.Abstractions;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;
using Hospital.Prescriptions.Application.Abstractions;
using Hospital.VitalSigns.Application.VitalSigns.Abstractions;

namespace Hospital.Api.ML;

public sealed class HostMlFeatureSource : IMlFeatureSource
{
    private readonly IPatientRepository _patients;
    private readonly IAdmissionRepository _admissions;
    private readonly IExamRepository _exams;
    private readonly IPrescriptionRepository _prescriptions;
    private readonly IVitalSignRepository _vitalSigns;
    private readonly IClinicalNoteRepository _clinicalNotes;
    private readonly IPatientAlertRepository _alerts;

    public HostMlFeatureSource(
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

    public async Task<MlFeatureVector?> GetPatientFeaturesAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await _patients.GetByIdAsync(
            new PatientId(patientId),
            cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - patient.BirthDate.Year;
        if (patient.BirthDate > today.AddYears(-age))
        {
            age--;
        }

        var admissions =
            await _admissions.GetByPatientIdAsync(patientId, cancellationToken);

        var activeAdmission = admissions
            .FirstOrDefault(a => a.Status.ToString() == "Active");

        var los = 0;
        if (activeAdmission is not null)
        {
            los = Math.Max(
                0,
                (int)(DateTimeOffset.UtcNow - activeAdmission.AdmissionDate).TotalDays);
        }

        var exams =
            await _exams.GetByPatientIdAsync(patientId, cancellationToken);

        var pendingExams = exams.Count(e =>
            e.Status.ToString() is "Requested" or "InProgress");

        var alerts =
            await _alerts.GetByPatientIdAsync(patientId, cancellationToken);

        var activeAlerts = alerts.Count(a => a.Status.ToString() == "Active");

        var prescriptions =
            await _prescriptions.GetByPatientIdAsync(patientId, cancellationToken);

        var activeRx = prescriptions.Count(p => p.Status.ToString() == "Active");

        var notes =
            await _clinicalNotes.GetByPatientIdAsync(patientId, cancellationToken);

        var hasMedical = notes.Any(n =>
            n.NoteType.ToString() is "Medical" or "Evolution");

        var vitals =
            await _vitalSigns.GetByPatientIdAsync(patientId, cancellationToken);

        var latest = vitals
            .OrderByDescending(v => v.MeasuredAtUtc)
            .FirstOrDefault();

        var spo2 = latest?.OxygenSaturation is null
            ? 97d
            : Convert.ToDouble(latest.OxygenSaturation, CultureInfo.InvariantCulture);

        var hr = latest?.HeartRate is null
            ? 80d
            : Convert.ToDouble(latest.HeartRate.Value, CultureInfo.InvariantCulture);

        return new MlFeatureVector(
            patientId,
            Math.Max(age, 0),
            los,
            activeAlerts,
            pendingExams,
            spo2,
            hr,
            activeRx,
            hasMedical);
    }
}
