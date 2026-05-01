using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.User.Data;
using Modules.User.Services;
using Modules.Tenant.Data;
using Modules.Tenant.Services;
using Modules.Audit.Data;
using Modules.Audit.Services;
using Modules.Permission.Data;
using Modules.Permission.Services;

var builder = WebApplication.CreateBuilder(args);

// Database connections
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb")));

builder.Services.AddDbContext<TenantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TenantDb")));

builder.Services.AddDbContext<AuditDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuditDb")));

builder.Services.AddDbContext<PermissionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PermissionDb")));

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

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
    context.Response.Headers.Append("X-BFF", "BFF-Portal");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Basic health check
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "bff-portal",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Liveness probe
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));

// Readiness probe - check all database connections
app.MapGet("/health/ready", async () =>
{
    var checks = new Dictionary<string, string>();
    var isReady = true;

    // Check User database
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        checks["user-db"] = canConnect ? "healthy" : "unhealthy";
        if (!canConnect) isReady = false;
    }
    catch (Exception ex)
    {
        checks["user-db"] = $"unhealthy: {ex.Message}";
        isReady = false;
    }

    // Check Tenant database
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        checks["tenant-db"] = canConnect ? "healthy" : "unhealthy";
        if (!canConnect) isReady = false;
    }
    catch (Exception ex)
    {
        checks["tenant-db"] = $"unhealthy: {ex.Message}";
        isReady = false;
    }

    // Check Audit database
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        checks["audit-db"] = canConnect ? "healthy" : "unhealthy";
        if (!canConnect) isReady = false;
    }
    catch (Exception ex)
    {
        checks["audit-db"] = $"unhealthy: {ex.Message}";
        isReady = false;
    }

    // Check Permission database
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PermissionDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync();
        checks["permission-db"] = canConnect ? "healthy" : "unhealthy";
        if (!canConnect) isReady = false;
    }
    catch (Exception ex)
    {
        checks["permission-db"] = $"unhealthy: {ex.Message}";
        isReady = false;
    }

    var result = new
    {
        status = isReady ? "ready" : "not_ready",
        service = "bff-portal",
        checks,
        timestamp = DateTime.UtcNow
    };

    return isReady ? Results.Ok(result) : Results.Json(result, statusCode: 503);
});

app.MapControllers();

app.Run();

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}