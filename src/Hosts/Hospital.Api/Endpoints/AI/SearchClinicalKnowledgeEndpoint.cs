using Hospital.AI.Application.Search;
using Hospital.AI.Contracts;
using Hospital.Api.Common;

namespace Hospital.Api.Endpoints.AI;

public static class SearchClinicalKnowledgeEndpoint
{
    public static IEndpointRouteBuilder MapSearchClinicalKnowledgeEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/ai/search",
                async (
                    SearchClinicalKnowledgeRequest request,
                    SearchClinicalKnowledgeHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await handler.HandleAsync(
                            new SearchClinicalKnowledgeQuery(
                                request.Query,
                                request.PatientId,
                                request.TopK),
                            cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.ToProblem();
                    }

                    var value = result.Value;

                    return Results.Ok(
                        new SearchClinicalKnowledgeResponse(
                            value.PatientId,
                            value.Query,
                            value.Hits
                                .Select(h => new ClinicalKnowledgeHitDto(
                                    h.SourceId,
                                    h.Title,
                                    h.Excerpt,
                                    h.Score))
                                .ToList()));
                })
            .WithTags("AI")
            .WithName("SearchClinicalKnowledge")
            .WithSummary("Busca semântica no prontuário indexado")
            .WithDescription(
                "Recupera evidências por similaridade (pgvector), filtradas pelo PatientId.")
            .Produces<SearchClinicalKnowledgeResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
