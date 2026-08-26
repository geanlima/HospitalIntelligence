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
                async (
                    LoginRequest request,
                    IOptions<SecurityOptions> options,
                    ISecurityAuditTrail auditTrail,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    var user = ResolveDemoUser(request.Username, request.Password);

                    if (user is null)
                    {
                        await auditTrail.AppendAsync(
                            new AuditTrailEntry(
                                Guid.NewGuid(),
                                DateTimeOffset.UtcNow,
                                request.Username,
                                "LoginFailed",
                                "/auth/login",
                                httpContext.Items["CorrelationId"]?.ToString()
                                    ?? httpContext.TraceIdentifier,
                                "invalid credentials"),
                            cancellationToken);

                        return Results.Unauthorized();
                    }

                    var token = SecurityExtensions.IssueDevToken(
                        options.Value,
                        user.Value.Username,
                        user.Value.Roles);

                    await auditTrail.AppendAsync(
                        new AuditTrailEntry(
                            Guid.NewGuid(),
                            DateTimeOffset.UtcNow,
                            user.Value.Username,
                            "Login",
                            "/auth/login",
                            httpContext.Items["CorrelationId"]?.ToString()
                                ?? httpContext.TraceIdentifier,
                            $"roles={string.Join(',', user.Value.Roles)}"),
                        cancellationToken);

                    return Results.Ok(token);
                })
            .AllowAnonymous()
            .WithTags("Security")
            .WithName("Login")
            .WithSummary("Login JWT (usuários demo: admin/admin, clinician/clinician, auditor/auditor)")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        app.MapGet(
                "/auth/me",
                (HttpContext httpContext) =>
                {
                    var user = httpContext.User;
                    if (user.Identity?.IsAuthenticated != true)
                    {
                        return Results.Unauthorized();
                    }

                    var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToArray();

                    return Results.Ok(
                        new
                        {
                            username = user.Identity.Name,
                            roles
                        });
                })
            .WithTags("Security")
            .WithName("GetCurrentUser")
            .WithSummary("Usuário autenticado atual");

        app.MapGet(
                "/auth/audit-trail",
                async (
                    ISecurityAuditTrail auditTrail,
                    CancellationToken cancellationToken) =>
                    Results.Ok(await auditTrail.ListAsync(100, cancellationToken)))
            .RequireAuthorization("AuditorOrAdmin")
            .WithTags("Security")
            .WithName("GetSecurityAuditTrail")
            .WithSummary("Trilha de auditoria de segurança (persistida no Postgres)");

        app.MapPost(
                "/lgpd/anonymize-name",
                async (
                    AnonymizeNameRequest request,
                    ISecurityAuditTrail auditTrail,
                    HttpContext httpContext,
                    CancellationToken cancellationToken) =>
                {
                    var anonymized = LgpdAnonymizer.AnonymizeName(request.Name);
                    var hash = LgpdAnonymizer.HashIdentifier(request.Name);

                    await auditTrail.AppendAsync(
                        new AuditTrailEntry(
                            Guid.NewGuid(),
                            DateTimeOffset.UtcNow,
                            httpContext.User.Identity?.Name,
                            "LgpdAnonymize",
                            "/lgpd/anonymize-name",
                            httpContext.Items["CorrelationId"]?.ToString()
                                ?? httpContext.TraceIdentifier,
                            $"hash={hash}"),
                        cancellationToken);

                    return Results.Ok(
                        new
                        {
                            originalLength = request.Name.Length,
                            anonymized,
                            hash
                        });
                })
            .RequireAuthorization("AuditorOrAdmin")
            .WithTags("LGPD")
            .WithName("AnonymizeName")
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
