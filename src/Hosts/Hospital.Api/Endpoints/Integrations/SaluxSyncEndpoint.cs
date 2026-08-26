using Hospital.Salux.Sync;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Api.Endpoints.Integrations;

public static class SaluxSyncEndpoint
{
    public static IEndpointRouteBuilder MapSaluxSyncEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/integrations/salux/patients/sync",
                async (
                    SaluxPatientSyncService syncService,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await syncService.SyncOnceAsync(cancellationToken);

                    return Results.Ok(result);
                })
            .RequireAuthorization("ClinicianOrAdmin")
            .WithTags("Integrations")
            .WithName("SyncSaluxPatients")
            .WithSummary("Dispara sync incremental de pacientes do Salux (read-only)")
            .Produces<SaluxSyncResult>(StatusCodes.Status200OK);

        return app;
    }
}
