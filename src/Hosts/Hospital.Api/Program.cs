using Hospital.Admissions.Application.Admissions.CreateAdmission;
using Hospital.Admissions.Application.Admissions.DischargeAdmission;
using Hospital.Admissions.Application.Admissions.SearchAdmissions;
using Hospital.Admissions.Infrastructure;

using Hospital.AI.Application.Abstractions;
using Hospital.AI.Infrastructure;

using Hospital.Alerts.Application.Alerts.AcknowledgeAlert;
using Hospital.Alerts.Application.Alerts.CreateAlert;
using Hospital.Alerts.Application.Alerts.ResolveAlert;
using Hospital.Alerts.Application.Alerts.SearchAlerts;
using Hospital.Alerts.Infrastructure;

using Hospital.Api.AI;
using Hospital.Api.CommandCenter;
using Hospital.Api.Common;
using Hospital.Api.Endpoints.Admissions;
using Hospital.Api.Endpoints.AI;
using Hospital.Api.Endpoints.Alerts;
using Hospital.Api.Endpoints.ClinicalNotes;
using Hospital.Api.Endpoints.CommandCenter;
using Hospital.Api.Endpoints.Dashboard;
using Hospital.Api.Endpoints.Exams;
using Hospital.Api.Endpoints.Integrations;
using Hospital.Api.Endpoints.ML;
using Hospital.Api.Endpoints.Patients;
using Hospital.Api.Endpoints.Prescriptions;
using Hospital.Api.Endpoints.Security;
using Hospital.Api.Endpoints.Timeline;
using Hospital.Api.Endpoints.VitalSigns;
using Hospital.Api.ML;
using Hospital.Api.Observability;
using Hospital.Api.Security;

using Hospital.ClinicalNotes.Application.ClinicalNotes.CreateClinicalNote;
using Hospital.ClinicalNotes.Application.ClinicalNotes.SearchClinicalNotes;
using Hospital.ClinicalNotes.Infrastructure;

using Hospital.Dashboard.Application.Dashboard;

using Hospital.Exams.Application.Exams.CreateExam;
using Hospital.Exams.Application.Exams.RegisterExamResult;
using Hospital.Exams.Application.Exams.SearchExams;
using Hospital.Exams.Infrastructure;

using Hospital.ML.Application.Abstractions;
using Hospital.ML.Infrastructure;

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

using Hospital.Salux;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddHospitalObservability();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHospitalSecurity(builder.Configuration);

builder.Services.AddPatientsInfrastructure(builder.Configuration);
builder.Services.AddAdmissionsInfrastructure(builder.Configuration);
builder.Services.AddExamsInfrastructure(builder.Configuration);
builder.Services.AddPrescriptionsInfrastructure(builder.Configuration);
builder.Services.AddVitalSignsInfrastructure(builder.Configuration);
builder.Services.AddClinicalNotesInfrastructure(builder.Configuration);
builder.Services.AddAlertsInfrastructure(builder.Configuration);
builder.Services.AddTimelineInfrastructure(builder.Configuration);
builder.Services.AddAiInfrastructure(builder.Configuration);
builder.Services.AddMlInfrastructure();
builder.Services.AddSaluxIntegration(builder.Configuration);

builder.Services.AddScoped<IClinicalRecordSource, HostClinicalRecordSource>();
builder.Services.AddScoped<IMlFeatureSource, HostMlFeatureSource>();

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
    builder.Configuration.GetSection(HospitalCapacityOptions.SectionName));

builder.Services.AddScoped<GetDashboardSummaryHandler>();
builder.Services.AddScoped<GetCommandCenterSummaryHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHospitalCorrelationId();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapAuthEndpoints();
app.MapSaluxSyncEndpoint();
app.MapDashboardSummaryEndpoint();
app.MapCommandCenterSummaryEndpoint();
app.MapPatientMlInsightsEndpoint();

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
app.MapIndexPatientClinicalRecordsEndpoint();
app.MapSearchClinicalKnowledgeEndpoint();
app.MapAuditPatientChartEndpoint();
app.MapAssessClinicalSafetyEndpoint();
app.MapStructureVoiceNoteEndpoint();

await app.Services.EnsureSecuritySchemaAsync();
await app.Services.EnsureSaluxSchemaAsync();
await app.Services.SeedAiKnowledgeAsync();

try
{
    app.Run();
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program;
