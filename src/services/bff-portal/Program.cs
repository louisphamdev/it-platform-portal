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
var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? "default-secret-change-in-production";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "it-platform";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "it-platform-portal";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "bff-portal" }));

// User endpoints
app.MapGet("/api/users", () => Results.Ok(new { message = "users endpoint" })).RequireAuthorization();
app.MapGet("/api/tenants", () => Results.Ok(new { message = "tenants endpoint" })).RequireAuthorization();
app.MapGet("/api/audit", () => Results.Ok(new { message = "audit endpoint" })).RequireAuthorization();
app.MapGet("/api/permissions", () => Results.Ok(new { message = "permissions endpoint" })).RequireAuthorization();

app.Run();
