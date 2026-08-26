using Hospital.Api.Security;
using Microsoft.Extensions.Options;

namespace Hospital.Api.Endpoints.Security;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/auth/login",
                (
                    LoginRequest request,
                    IOptions<SecurityOptions> options,
                    ISecurityAuditTrail auditTrail,
                    HttpContext httpContext) =>
                {
                    var user = ResolveDemoUser(request.Username, request.Password);

                    if (user is null)
                    {
                        return Results.Unauthorized();
                    }

                    var token = SecurityExtensions.IssueDevToken(
                        options.Value,
                        user.Value.Username,
                        user.Value.Roles);

                    auditTrail.Append(
                        new AuditTrailEntry(
                            Guid.NewGuid(),
                            DateTimeOffset.UtcNow,
                            user.Value.Username,
                            "Login",
                            "/auth/login",
                            httpContext.TraceIdentifier,
                            $"roles={string.Join(',', user.Value.Roles)}"));

                    return Results.Ok(token);
                })
            .AllowAnonymous()
            .WithTags("Security")
            .WithName("Login")
            .WithSummary("Login JWT de estudo (usuários demo)")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        app.MapGet(
                "/auth/audit-trail",
                (ISecurityAuditTrail auditTrail) =>
                    Results.Ok(auditTrail.List()))
            .WithTags("Security")
            .WithName("GetSecurityAuditTrail")
            .AllowAnonymous()
            .WithSummary("Trilha de auditoria de segurança (estudo; proteger com RequireAuth em prod)");

        app.MapPost(
                "/lgpd/anonymize-name",
                (AnonymizeNameRequest request) =>
                    Results.Ok(
                        new
                        {
                            originalLength = request.Name.Length,
                            anonymized = LgpdAnonymizer.AnonymizeName(request.Name),
                            hash = LgpdAnonymizer.HashIdentifier(request.Name)
                        }))
            .WithTags("LGPD")
            .WithName("AnonymizeName")
            .AllowAnonymous()
            .WithSummary("Anonimização/pseudonimização de estudo (LGPD)");

        return app;
    }

    private static (string Username, IReadOnlyList<string> Roles)? ResolveDemoUser(
        string username,
        string password)
    {
        // Senhas apenas para estudo local — nunca usar em produção.
        return (username.Trim().ToLowerInvariant(), password) switch
        {
            ("admin", "admin") =>
                ("admin", new[] { HospitalRoles.Admin, HospitalRoles.Clinician }),
            ("clinician", "clinician") =>
                ("clinician", new[] { HospitalRoles.Clinician }),
            ("auditor", "auditor") =>
                ("auditor", new[] { HospitalRoles.Auditor }),
            _ => null
        };
    }
}

public sealed record AnonymizeNameRequest(string Name);
