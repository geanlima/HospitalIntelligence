using Hospital.Admissions.Application.Admissions.CreateAdmission;
using Hospital.Admissions.Application.Admissions.DischargeAdmission;
using Hospital.Admissions.Application.Admissions.SearchAdmissions;
using Hospital.Admissions.Infrastructure;

using Hospital.AI.Infrastructure;

using Hospital.Alerts.Application.Alerts.AcknowledgeAlert;
using Hospital.Alerts.Application.Alerts.CreateAlert;
using Hospital.Alerts.Application.Alerts.ResolveAlert;
using Hospital.Alerts.Application.Alerts.SearchAlerts;
using Hospital.Alerts.Infrastructure;

using Hospital.Api.Common;
using Hospital.Api.Endpoints.Admissions;
using Hospital.Api.Endpoints.AI;
using Hospital.Api.Endpoints.Alerts;
using Hospital.Api.Endpoints.ClinicalNotes;
using Hospital.Api.Endpoints.Dashboard;
using Hospital.Api.Endpoints.Exams;
using Hospital.Api.Endpoints.Patients;
using Hospital.Api.Endpoints.Prescriptions;
using Hospital.Api.Endpoints.Timeline;
using Hospital.Api.Endpoints.VitalSigns;

using Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;
using Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;
using Hospital.ClinicalNotes.Infrastructure;

using Hospital.Dashboard.Application.Dashboard;

using Hospital.Exams.Application.Exams.CreateExam;
using Hospital.Exams.Application.Exams.RegisterExamResult;
using Hospital.Exams.Application.Exams.SearchExams;
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
using Hospital.Prescriptions.Application.SearchPrescriptions;
using Hospital.Prescriptions.Infrastructure;

using Hospital.Timeline.Application.Timeline.CreateTimelineItem;
using Hospital.Timeline.Infrastructure;

using Hospital.VitalSigns.Application.VitalSigns.CreateVitalSign;
using Hospital.VitalSigns.Application.VitalSigns.SearchVitalSigns;
using Hospital.VitalSigns.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

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

builder.Services.AddAiInfrastructure(
    builder.Configuration);

builder.Services.AddScoped<CreatePatientHandler>();
builder.Services.AddScoped<GetPatientByIdHandler>();
builder.Services.AddScoped<SearchPatientsHandler>();
builder.Services.AddScoped<UpdatePatientHandler>();
builder.Services.AddScoped<SynchronizeExternalPatientHandler>();
builder.Services.AddScoped<GetPatient360Handler>();

builder.Services.AddScoped<CreateAdmissionHandler>();
builder.Services.AddScoped<DischargeAdmissionHandler>();
builder.Services.AddScoped<SearchAdmissionsHandler>();

builder.Services.AddScoped<CreateExamHandler>();
builder.Services.AddScoped<RegisterExamResultHandler>();
builder.Services.AddScoped<SearchExamsHandler>();

builder.Services.AddScoped<CreatePrescriptionHandler>();
builder.Services.AddScoped<ChangePrescriptionStatusHandler>();
builder.Services.AddScoped<SearchPrescriptionsHandler>();

builder.Services.AddScoped<CreateVitalSignHandler>();
builder.Services.AddScoped<SearchVitalSignsHandler>();

builder.Services.AddScoped<CreateClinicalNoteHandler>();
builder.Services.AddScoped<SearchClinicalNotesHandler>();

builder.Services.AddScoped<CreateAlertHandler>();
builder.Services.AddScoped<AcknowledgeAlertHandler>();
builder.Services.AddScoped<ResolveAlertHandler>();
builder.Services.AddScoped<SearchAlertsHandler>();

builder.Services.AddScoped<CreateTimelineItemHandler>();

builder.Services.Configure<HospitalCapacityOptions>(
    builder.Configuration.GetSection(
        HospitalCapacityOptions.SectionName));

builder.Services.AddScoped<GetDashboardSummaryHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapDashboardSummaryEndpoint();

app.MapCreatePatientEndpoint();
app.MapGetPatientByIdEndpoint();
app.MapSearchPatientsEndpoint();
app.MapUpdatePatientEndpoint();
app.MapSynchronizeExternalPatientEndpoint();
app.MapGetPatient360Endpoint();

app.MapCreateAdmissionEndpoint();
app.MapDischargeAdmissionEndpoint();
app.MapSearchAdmissionsEndpoint();

app.MapCreateExamEndpoint();
app.MapRegisterExamResultEndpoint();
app.MapSearchExamsEndpoint();

app.MapCreatePrescriptionEndpoint();
app.MapChangePrescriptionStatusEndpoint();
app.MapSearchPrescriptionsEndpoint();

app.MapCreateVitalSignEndpoint();
app.MapSearchVitalSignsEndpoint();

app.MapCreateClinicalNoteEndpoint();
app.MapSearchClinicalNotesEndpoint();

app.MapCreateAlertEndpoint();
app.MapAcknowledgeAlertEndpoint();
app.MapResolveAlertEndpoint();
app.MapSearchAlertsEndpoint();

app.MapCreateTimelineItemEndpoint();

app.MapAskAiEndpoint();

await app.Services.SeedAiKnowledgeAsync();

app.Run();

public partial class Program;
