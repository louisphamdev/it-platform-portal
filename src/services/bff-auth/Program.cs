using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Data;
using Modules.Auth.Services;
using Bff.Auth;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-BFF", "BFF-Auth");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Basic health check
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "bff-auth",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Liveness probe
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));

// Readiness probe - check database connectivity
app.MapGet("/health/ready", async () =>
{
    var checks = new Dictionary<string, string>();
    var isReady = true;

    // Check database connection
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        checks["database"] = canConnect ? "healthy" : "unhealthy";
        if (!canConnect) isReady = false;
    }
    catch (Exception ex)
    {
        checks["database"] = $"unhealthy: {ex.Message}";
        isReady = false;
    }

    var result = new
    {
        status = isReady ? "ready" : "not_ready",
        service = "bff-auth",
        checks,
        timestamp = DateTime.UtcNow
    };

    return isReady ? Results.Ok(result) : Results.Json(result, statusCode: 503);
});

app.MapControllers();

app.Run();