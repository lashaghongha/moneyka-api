using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MoneyKa.Api.Services;

namespace MoneyKa.Api.Filters;

/// <summary>
/// Authorizes Moneyka admin requests via either the new httpOnly admin_token session
/// cookie (minted by the Admin Hub SSO consume flow, validated by AdminSessionService)
/// or the legacy X-Admin-Key header. Either credential grants access, so the existing
/// static-key callers keep working while SSO sessions are honoured.
/// </summary>
public class AdminAuthFilter(IConfiguration config, AdminSessionService session) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        var request = ctx.HttpContext.Request;

        if (request.Cookies.TryGetValue("admin_token", out var cookie) && session.Validate(cookie))
            return;

        var key = config["Admin:Key"] ?? "moneykaadmin2024";
        if (request.Headers.TryGetValue("X-Admin-Key", out var val) && val == key)
            return;

        ctx.Result = new UnauthorizedObjectResult(new { error = "Admin key required" });
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthAttribute : TypeFilterAttribute
{
    public AdminAuthAttribute() : base(typeof(AdminAuthFilter)) { }
}
