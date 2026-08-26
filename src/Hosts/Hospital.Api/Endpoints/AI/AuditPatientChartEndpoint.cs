using Hospital.AI.Application.Audit;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class AuditPatientChartEndpoint
{
    public static IEndpointRouteBuilder MapAuditPatientChartEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/audit/patients/{patientId:guid}",
                async (
                    Guid patientId,
                    AuditPatientChartHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new AuditPatientChartQuery(patientId),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new AuditPatientChartResponse(
                            value.PatientId,
                            value.AuditedAtUtc,
                            value.OverallRisk,
                            value.Summary,
                            value.MissingDocumentationCount,
                            value.DivergenceCount,
                            value.FinancialGlosaRiskCount,
                            value.Findings
                                .Select(f => new ChartAuditFindingDto(
                                    f.Code,
                                    f.Category,
                                    f.Severity,
                                    f.Title,
                                    f.Message,
                                    f.RelatedSourceIds))
                                .ToList()));
                })
            .WithTags("AI")
            .WithName("AuditPatientChart")
            .WithSummary("Audita o prontuário do paciente")
            .WithDescription(
                "Avalia documentação ausente, divergências e risco de glosa " +
                "com regras determinísticas sobre o prontuário canônico.")
            .Produces<AuditPatientChartResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
