using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using YARP.Gateway.Configuration;
using YARP.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

// Add Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms<JwtTokenTransformProvider>();

// Add Authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Gateway", "ITPlatformPortal-API-Gateway");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint with detailed status
app.MapGet("/health", async () =>
{
    var health = new
    {
        status = "healthy",
        service = "api-gateway",
        timestamp = DateTime.UtcNow,
        version = "1.0.0"
    };
    return Results.Ok(health);
});

// Liveness probe
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));

// Readiness probe - check if backend services are reachable
app.MapGet("/health/ready", async () =>
{
    var isReady = true;
    var checks = new Dictionary<string, string>();

    // Check BFF Auth
    try
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(3);
        var response = await client.GetAsync("http://bff-auth:7001/health");
        checks["bff-auth"] = response.IsSuccessStatusCode ? "healthy" : "unhealthy";
        if (!response.IsSuccessStatusCode) isReady = false;
    }
    catch
    {
        checks["bff-auth"] = "unreachable";
        isReady = false;
    }

    // Check BFF Portal
    try
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(3);
        var response = await client.GetAsync("http://bff-portal:7002/health");
        checks["bff-portal"] = response.IsSuccessStatusCode ? "healthy" : "unhealthy";
        if (!response.IsSuccessStatusCode) isReady = false;
    }
    catch
    {
        checks["bff-portal"] = "unreachable";
        isReady = false;
    }

    var result = new
    {
        status = isReady ? "ready" : "not_ready",
        service = "api-gateway",
        checks,
        timestamp = DateTime.UtcNow
    };

    return isReady ? Results.Ok(result) : Results.Json(result, statusCode: 503);
});

app.MapReverseProxy();

app.Run();

// JWT Token Transform for YARP - adds Authorization header from cookie or validates token
public class JwtTokenTransformProvider : ITransformProvider
{
    public void ValidateRoute(TransformRouteContext routeContext) { }

    public void ValidateCluster(TransformClusterContext clusterContext) { }

    public IRequestTransform? CreateRequestTransform(RequestTransformContext context)
    {
        return new JwtTokenTransform();
    }
}

public class JwtTokenTransform : IRequestTransform
{
    public ValueTask ApplyAsync(RequestTransformContext context)
    {
        // Check for token in query string (for WebSocket upgrades or special cases)
        if (context.Query.Collection.TryGetValue("access_token", out var tokenValues))
        {
            var token = tokenValues.FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                context.ProxyRequest.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        return ValueTask.CompletedTask;
    }
}