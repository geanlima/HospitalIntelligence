using Hospital.Admissions.Application.Admissions.CreateAdmission;
using Hospital.Admissions.Application.Admissions.DischargeAdmission;
using Hospital.Admissions.Infrastructure;

using Hospital.Alerts.Application.Alerts.AcknowledgeAlert;
using Hospital.Alerts.Application.Alerts.CreateAlert;
using Hospital.Alerts.Application.Alerts.ResolveAlert;
using Hospital.Alerts.Infrastructure;

using Hospital.Api.Common;
using Hospital.Api.Endpoints.Admissions;
using Hospital.Api.Endpoints.Alerts;
using Hospital.Api.Endpoints.ClinicalNotes;
using Hospital.Api.Endpoints.Exams;
using Hospital.Api.Endpoints.Patients;
using Hospital.Api.Endpoints.Prescriptions;
using Hospital.Api.Endpoints.Timeline;
using Hospital.Api.Endpoints.VitalSigns;

using Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;
using Hospital.ClinicalNotes.Infrastructure;

using Hospital.Exams.Application.Exams.CreateExam;
using Hospital.Exams.Application.Exams.RegisterExamResult;
using Hospital.Exams.Infrastructure;

using Hospital.Patients.Application.Patient360;
using Hospital.Patients.Application.Patients.CreatePatient;
using Hospital.Patients.Application.Patients.GetPatientById;
using Hospital.Patients.Application.Patients.SearchPatients;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Application.Patients.UpdatePatient;
using Hospital.Patients.Infrastructure;

using Hospital.Prescriptions.Application.ChangePrescriptionStatus;
using Hospital.Prescriptions.Application.CreatePrescription;
using Hospital.Prescriptions.Infrastructure;

using Hospital.Timeline.Application.Timeline.CreateTimelineItem;
using Hospital.Timeline.Infrastructure;

using Hospital.VitalSigns.Application.VitalSigns.CreateVitalSign;
using Hospital.VitalSigns.Infrastructure;

var builder =
    WebApplication.CreateBuilder(args);

//
// Swagger
//

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// CORS
//

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//
// Exception Handling
//

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//
// Infrastructure
//

builder.Services.AddPatientsInfrastructure(
    builder.Configuration);

builder.Services.AddAdmissionsInfrastructure(
    builder.Configuration);

builder.Services.AddExamsInfrastructure(
    builder.Configuration);

builder.Services.AddPrescriptionsInfrastructure(
    builder.Configuration);

builder.Services.AddVitalSignsInfrastructure(
    builder.Configuration);

builder.Services.AddClinicalNotesInfrastructure(
    builder.Configuration);

builder.Services.AddAlertsInfrastructure(
    builder.Configuration);

builder.Services.AddTimelineInfrastructure(
    builder.Configuration);

//
// Patients
//

builder.Services.AddScoped<CreatePatientHandler>();
builder.Services.AddScoped<GetPatientByIdHandler>();
builder.Services.AddScoped<SearchPatientsHandler>();
builder.Services.AddScoped<UpdatePatientHandler>();
builder.Services.AddScoped<SynchronizeExternalPatientHandler>();
builder.Services.AddScoped<GetPatient360Handler>();

//
// Admissions
//

builder.Services.AddScoped<CreateAdmissionHandler>();
builder.Services.AddScoped<DischargeAdmissionHandler>();

//
// Exams
//

builder.Services.AddScoped<CreateExamHandler>();
builder.Services.AddScoped<RegisterExamResultHandler>();

//
// Prescriptions
//

builder.Services.AddScoped<CreatePrescriptionHandler>();
builder.Services.AddScoped<ChangePrescriptionStatusHandler>();

//
// Vital Signs
//

builder.Services.AddScoped<CreateVitalSignHandler>();

//
// Clinical Notes
//

builder.Services.AddScoped<CreateClinicalNoteHandler>();

//
// Alerts
//

builder.Services.AddScoped<CreateAlertHandler>();
builder.Services.AddScoped<AcknowledgeAlertHandler>();
builder.Services.AddScoped<ResolveAlertHandler>();

//
// Timeline
//

builder.Services.AddScoped<CreateTimelineItemHandler>();

var app =
    builder.Build();

//
// Middleware
//

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//
// CORS
//

app.UseCors("Frontend");

//
// Patients
//

app.MapCreatePatientEndpoint();
app.MapGetPatientByIdEndpoint();
app.MapSearchPatientsEndpoint();
app.MapUpdatePatientEndpoint();
app.MapSynchronizeExternalPatientEndpoint();
app.MapGetPatient360Endpoint();

//
// Admissions
//

app.MapCreateAdmissionEndpoint();
app.MapDischargeAdmissionEndpoint();

//
// Exams
//

app.MapCreateExamEndpoint();
app.MapRegisterExamResultEndpoint();

//
// Prescriptions
//

app.MapCreatePrescriptionEndpoint();
app.MapChangePrescriptionStatusEndpoint();

//
// Vital Signs
//

app.MapCreateVitalSignEndpoint();

//
// Clinical Notes
//

app.MapCreateClinicalNoteEndpoint();

//
// Alerts
//

app.MapCreateAlertEndpoint();
app.MapAcknowledgeAlertEndpoint();
app.MapResolveAlertEndpoint();

//
// Timeline
//

app.MapCreateTimelineItemEndpoint();

app.Run();

public partial class Program;