using Hospital.Api.Common;
using Hospital.Api.Endpoints.Patients;
using Hospital.Patients.Application.Patients.CreatePatient;
using Hospital.Patients.Application.Patients.GetPatientById;
using Hospital.Patients.Application.Patients.SearchPatients;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Application.Patients.UpdatePatient;
using Hospital.Patients.Infrastructure;

var builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddPatientsInfrastructure(
    builder.Configuration);

builder.Services.AddScoped<CreatePatientHandler>();
builder.Services.AddScoped<GetPatientByIdHandler>();
builder.Services.AddScoped<SearchPatientsHandler>();
builder.Services.AddScoped<UpdatePatientHandler>();
builder.Services.AddScoped<SynchronizeExternalPatientHandler>();

var app =
    builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCreatePatientEndpoint();
app.MapGetPatientByIdEndpoint();
app.MapSearchPatientsEndpoint();
app.MapUpdatePatientEndpoint();
app.MapSynchronizeExternalPatientEndpoint();

app.Run();

public partial class Program;