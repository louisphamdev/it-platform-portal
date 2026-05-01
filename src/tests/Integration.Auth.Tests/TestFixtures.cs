using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Data;
using Modules.Auth.Models;

namespace Integration.Auth.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Add in-memory database for testing
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestAuthDb_" + Guid.NewGuid().ToString("N"));
            });
        });

        builder.UseEnvironment("Testing");
    }
}

public static class TestJwtGenerator
{
    private const string TestSecretKey = "TestSecretKeyForIntegrationTests123456789012345678901234567890";
    private const string TestIssuer = "ITPlatformPortal";
    private const string TestAudience = "ITPlatformPortal";

    public static string GenerateAccessToken(Guid userId, string username, string email, List<string> roles, Guid? tenantId = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Email, email),
            new("tenant_id", tenantId?.ToString() ?? string.Empty)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRefreshToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("token_type", "refresh")
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public static ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TestSecretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = TestIssuer,
                ValidAudience = TestAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

public static class TestDataSeeder
{
    public static async Task<(User user, Role adminRole)> SeedTestUserWithRole(AuthDbContext context)
    {
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "Administrator role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "testadmin",
            Email = "testadmin@test.local",
            PasswordHash = TestJwtGenerator.HashPassword("TestPassword123!"),
            FirstName = "Test",
            LastName = "Admin",
            IsActive = true,
            IsLocked = false,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var userRole = new UserRole
        {
            UserId = testUser.Id,
            RoleId = adminRole.Id
        };

        context.Roles.Add(adminRole);
        context.Users.Add(testUser);
        context.UserRoles.Add(userRole);
        await context.SaveChangesAsync();

        return (testUser, adminRole);
    }

    public static async Task<User> SeedLockedUser(AuthDbContext context)
    {
        var lockedUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "lockeduser",
            Email = "locked@test.local",
            PasswordHash = TestJwtGenerator.HashPassword("TestPassword123!"),
            FirstName = "Locked",
            LastName = "User",
            IsActive = true,
            IsLocked = true,
            FailedLoginAttempts = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(lockedUser);
        await context.SaveChangesAsync();

        return lockedUser;
    }
}
