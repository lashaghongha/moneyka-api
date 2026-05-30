using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoneyKa.Api.Filters;

public class AdminAuthFilter(IConfiguration config) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        var key = config["Admin:Key"] ?? "moneykaadmin2024";
        if (!ctx.HttpContext.Request.Headers.TryGetValue("X-Admin-Key", out var val) || val != key)
            ctx.Result = new UnauthorizedObjectResult(new { error = "Admin key required" });
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AdminAuthAttribute : TypeFilterAttribute
{
    public AdminAuthAttribute() : base(typeof(AdminAuthFilter)) { }
}
