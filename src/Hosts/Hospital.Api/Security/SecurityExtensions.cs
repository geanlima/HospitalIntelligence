using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Hospital.Api.Security;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public bool RequireAuth { get; set; }

    public string JwtIssuer { get; set; } = "HospitalIntelligence";

    public string JwtAudience { get; set; } = "HospitalIntelligence";

    public string JwtSigningKey { get; set; } =
        "dev-only-change-me-hospital-intelligence-32chars!";

    public int TokenLifetimeMinutes { get; set; } = 480;
}

public static class HospitalRoles
{
    public const string Admin = "Admin";
    public const string Clinician = "Clinician";
    public const string Auditor = "Auditor";
}

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string Username,
    IReadOnlyList<string> Roles);

public sealed record AuditTrailEntry(
    Guid Id,
    DateTimeOffset OccurredAtUtc,
    string? UserName,
    string Action,
    string Resource,
    string? CorrelationId,
    string Details);

public interface ISecurityAuditTrail
{
    void Append(AuditTrailEntry entry);

    IReadOnlyList<AuditTrailEntry> List(int take = 100);
}

public sealed class InMemorySecurityAuditTrail : ISecurityAuditTrail
{
    private readonly List<AuditTrailEntry> _entries = [];
    private readonly object _gate = new();

    public void Append(AuditTrailEntry entry)
    {
        lock (_gate)
        {
            _entries.Insert(0, entry);
            if (_entries.Count > 1000)
            {
                _entries.RemoveRange(1000, _entries.Count - 1000);
            }
        }
    }

    public IReadOnlyList<AuditTrailEntry> List(int take = 100)
    {
        lock (_gate)
        {
            return _entries.Take(take).ToList();
        }
    }
}

public static class LgpdAnonymizer
{
    public static string AnonymizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "ANON";
        }

        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(
            ' ',
            parts.Select(p => p[0] + "***"));
    }

    public static string HashIdentifier(string value)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(bytes)[..16];
    }
}

public static class SecurityExtensions
{
    public static IServiceCollection AddHospitalSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SecurityOptions>(
            configuration.GetSection(SecurityOptions.SectionName));

        var options =
            configuration
                .GetSection(SecurityOptions.SectionName)
                .Get<SecurityOptions>() ?? new SecurityOptions();

        services.AddSingleton<ISecurityAuditTrail, InMemorySecurityAuditTrail>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = options.JwtIssuer,
                    ValidAudience = options.JwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(options.JwtSigningKey))
                };
            });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy(
                "ClinicianOrAdmin",
                p => p.RequireRole(HospitalRoles.Clinician, HospitalRoles.Admin));

            auth.AddPolicy(
                "AuditorOrAdmin",
                p => p.RequireRole(HospitalRoles.Auditor, HospitalRoles.Admin));
        });

        return services;
    }

    public static LoginResponse IssueDevToken(
        SecurityOptions options,
        string username,
        IReadOnlyList<string> roles)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(options.TokenLifetimeMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Sub, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.JwtSigningKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.JwtIssuer,
            options.JwtAudience,
            claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expires,
            username,
            roles);
    }
}
