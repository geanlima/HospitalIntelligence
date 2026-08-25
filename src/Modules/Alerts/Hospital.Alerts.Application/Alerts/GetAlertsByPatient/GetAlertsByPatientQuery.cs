namespace Hospital.Alerts.Application.Alerts.GetAlertsByPatient;

public sealed record GetAlertsByPatientQuery(
    Guid PatientId);