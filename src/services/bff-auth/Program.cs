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
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "bff-auth" }));

// Auth endpoints
app.MapPost("/auth/login", async (IAuthService authService, HttpContext context) =>
{
    var body = await context.Request.ReadFromJsonAsync<LoginRequest>();
    if (body == null) return Results.BadRequest();
    
    var result = await authService.LoginAsync(body.Username, body.Password, body.TenantId);
    if (result == null) return Results.Unauthorized();
    
    return Results.Ok(result);
});

app.MapGet("/auth/me", async (HttpContext context, IAuthService authService) =>
{
    var userId = context.User.FindFirst("sub")?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();
    
    var user = await authService.GetUserByIdAsync(userId);
    if (user == null) return Results.NotFound();
    
    return Results.Ok(user);
}).RequireAuthorization();

app.Run();

public record LoginRequest(string Username, string Password, string? TenantId);
