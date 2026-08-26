using System.Diagnostics;
using Serilog;

namespace Hospital.Api.Observability;

public static class ObservabilityExtensions
{
    public const string CorrelationHeader = "X-Correlation-ID";

    public static readonly ActivitySource ActivitySource =
        new("Hospital.Api");

    public static WebApplicationBuilder AddHospitalObservability(
        this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Hospital.Api")
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();

        var patientsCs =
            builder.Configuration.GetConnectionString("PatientsDatabase")
            ?? builder.Configuration.GetConnectionString("AiDatabase");

        builder.Services
            .AddHealthChecks()
            .AddNpgSql(
                patientsCs
                ?? "Host=localhost;Database=hospital_intelligence;Username=postgres;Password=postgres",
                name: "postgres");

        return builder;
    }

    public static IApplicationBuilder UseHospitalCorrelationId(
        this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var correlationId =
                context.Request.Headers[CorrelationHeader].FirstOrDefault()
                ?? Activity.Current?.Id
                ?? Guid.NewGuid().ToString("N");

            context.Response.Headers[CorrelationHeader] = correlationId;
            context.Items["CorrelationId"] = correlationId;

            using var activity = ActivitySource.StartActivity(
                "http.request");

            activity?.SetTag("correlation.id", correlationId);

            using (Serilog.Context.LogContext.PushProperty(
                       "CorrelationId",
                       correlationId))
            {
                await next();
            }
        });
    }
}
