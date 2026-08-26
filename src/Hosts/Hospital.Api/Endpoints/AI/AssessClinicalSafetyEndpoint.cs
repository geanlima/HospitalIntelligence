using Hospital.AI.Application.ClinicalSafety;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class AssessClinicalSafetyEndpoint
{
    public static IEndpointRouteBuilder MapAssessClinicalSafetyEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/clinical-safety/patients/{patientId:guid}",
                async (
                    Guid patientId,
                    AssessClinicalSafetyHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new AssessClinicalSafetyQuery(patientId),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new AssessClinicalSafetyResponse(
                            value.PatientId,
                            value.AssessedAtUtc,
                            value.OverallRisk,
                            value.Summary,
                            value.DischargeReady,
                            value.DischargeBlockerCount,
                            value.DeteriorationScore,
                            value.DeteriorationBand,
                            value.TriageRecommendation,
                            value.MedicationIssueCount,
                            value.Findings
                                .Select(f => new ClinicalSafetyFindingDto(
                                    f.Code,
                                    f.Category,
                                    f.Severity,
                                    f.Title,
                                    f.Message,
                                    f.RelatedSourceIds))
                                .ToList()));
                })
            .WithTags("AI")
            .WithName("AssessClinicalSafety")
            .WithSummary("Avalia segurança clínica do paciente")
            .WithDescription(
                "Alta segura, NEWS2-lite, reconciliação medicamentosa textual e sugestão de triagem.")
            .Produces<AssessClinicalSafetyResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
