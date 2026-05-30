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
builder.Services.AddScoped<GroqService>();
builder.Services.AddScoped<PushService>();
builder.Services.AddSingleton<OtpService>();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
         .AllowAnyHeader()
         .AllowAnyMethod()));

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
