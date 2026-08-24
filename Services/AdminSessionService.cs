using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MoneyKa.Api.Services;

/// <summary>
/// Mints and validates Moneyka's admin session — an HMAC-SHA256 JWT delivered as an
/// httpOnly cookie. Moneyka historically had no session at all (admin access was a
/// static X-Admin-Key header); this exists so the Admin Hub SSO flow can drop a real
/// session cookie on a redirect, matching the other targets. The legacy header still
/// works (see AdminAuthFilter) so nothing existing breaks.
/// </summary>
public sealed class AdminSessionService
{
    public const string Issuer = "moneyka-admin";
    public const string Audience = "moneyka-admin";

    private readonly SymmetricSecurityKey _key;

    public AdminSessionService()
    {
        var secret = Environment.GetEnvironmentVariable("ADMIN_SESSION_SECRET")
                     ?? "dev-only-insecure-moneyka-admin-secret-change-me-32chars";
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    public string Issue()
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: new[] { new Claim(ClaimTypes.Role, "Admin") },
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool Validate(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
                ClockSkew = TimeSpan.FromSeconds(15),
            }, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
