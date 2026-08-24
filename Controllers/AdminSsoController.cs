using Microsoft.AspNetCore.Mvc;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Controllers;

/// <summary>
/// Consumes a single-use SSO ticket minted by the central Admin Hub and, on success,
/// drops Moneyka's own admin session as an httpOnly cookie — the same cookie
/// AdminAuthFilter honours. Moneyka has a single shared admin, so a valid hub ticket
/// for this audience is sufficient; there is no per-user lookup.
/// </summary>
[ApiController]
[Route("api/admin/sso")]
public sealed class AdminSsoController : ControllerBase
{
    private readonly SsoService _sso;
    private readonly AdminSessionService _session;
    private readonly ILogger<AdminSsoController> _logger;

    public AdminSsoController(SsoService sso, AdminSessionService session, ILogger<AdminSsoController> logger)
    {
        _sso = sso;
        _session = session;
        _logger = logger;
    }

    // GET /api/admin/sso/consume?token=...
    [HttpGet("consume")]
    public IActionResult Consume([FromQuery] string? token)
    {
        var appBase = (Environment.GetEnvironmentVariable("ADMIN_APP_URL") ?? "").TrimEnd('/');
        var loginUrl = string.IsNullOrEmpty(appBase) ? "/admin/login" : $"{appBase}/admin/login";
        var adminUrl = string.IsNullOrEmpty(appBase) ? "/admin" : $"{appBase}/admin";

        var result = _sso.Validate(token ?? "");
        if (!result.Ok)
        {
            _logger.LogWarning("SSO consume rejected: {Error}", result.Error);
            return Redirect($"{loginUrl}?sso={result.Error}");
        }

        SetAuthCookie(_session.Issue());
        _logger.LogInformation("SSO admin login succeeded for hub user {Username}", result.Username);
        return Redirect(adminUrl);
    }

    private void SetAuthCookie(string token)
    {
        var isHttps = Request.IsHttps;
        Response.Cookies.Append("admin_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddHours(8),
        });
    }
}
