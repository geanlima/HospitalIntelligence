using Hospital.AI.Application.Index;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class IndexPatientClinicalRecordsEndpoint
{
    public static IEndpointRouteBuilder MapIndexPatientClinicalRecordsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/index/patients/{patientId:guid}",
                async (
                    Guid patientId,
                    IndexPatientClinicalRecordsHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new IndexPatientClinicalRecordsCommand(patientId),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new IndexPatientClinicalRecordsResponse(
                            value.PatientId,
                            value.IndexedCount,
                            value.IndexedAtUtc));
                })
            .WithTags("AI")
            .WithName("IndexPatientClinicalRecords")
            .WithSummary("Indexa o prontuário do paciente no vector store")
            .WithDescription(
                "Lê registros clínicos canônicos e gera embeddings para busca semântica / RAG.")
            .Produces<IndexPatientClinicalRecordsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
