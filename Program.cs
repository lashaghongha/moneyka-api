using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoneyKa.Api.Data;
using MoneyKa.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=moneyka.db"));

builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<OpenAIService>();
builder.Services.AddScoped<PushService>();
builder.Services.AddSingleton<OtpService>();

// Admin Hub SSO: hub-ticket validator + single-use replay guard, plus the local
// httpOnly admin session minted after a ticket is consumed.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SsoService>();
builder.Services.AddSingleton<AdminSessionService>();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
        p.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5175",
            "https://moneyka.vercel.app",
            "https://moneyka-1yktrtduo-lashagongadze102-4608s-projects.vercel.app"
         )
         .AllowAnyHeader()
         .AllowAnyMethod()
         // Needed so the admin SPA's cross-origin cookie probe (checkSession) sends
         // and receives the httpOnly admin_token cookie. Valid because origins are
         // an explicit allowlist, not a wildcard.
         .AllowCredentials()));

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "MoneyKa API",
        Version     = "v1",
        Description = "Georgian Expense Tracker — .NET 9 Backend"
    });
});

var app = builder.Build();

// Honor X-Forwarded-Proto/-For from the hosting proxy (Railway terminates TLS
// and forwards plain HTTP to the app). Without this, Request.IsHttps is false in
// production, so the SSO admin cookie is issued as SameSite=Lax without Secure —
// which the browser then refuses to send on cross-site calls from the Vercel
// admin frontend, so the SSO login silently fails. Railway is the sole ingress,
// so the proxy list is cleared to trust the forwarded headers. MUST run first.
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedOptions);

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger UI → /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyKa API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();
app.MapControllers();
app.Run();
