using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Modules.User.Data;
using Modules.User.Models;

namespace Modules.User.Services;

public interface IUserService
{
    Task<UserProfile?> GetProfileAsync(Guid userId);
    Task<UserProfile?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<UserSession?> CreateSessionAsync(Guid userId, string refreshToken, string? deviceInfo, string? ipAddress);
    Task<bool> RevokeSessionAsync(string refreshToken);
    Task<bool> RevokeAllSessionsAsync(Guid userId);
    Task CleanupExpiredSessionsAsync();
}

public class UserService : IUserService
{
    private readonly UserDbContext _context;

    public UserService(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetProfileAsync(Guid userId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserProfile?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile { UserId = userId, CreatedAt = DateTime.UtcNow };
            _context.UserProfiles.Add(profile);
        }

        if (request.Department != null) profile.Department = request.Department;
        if (request.JobTitle != null) profile.JobTitle = request.JobTitle;
        if (request.Address != null) profile.Address = request.Address;
        if (request.City != null) profile.City = request.City;
        if (request.Country != null) profile.Country = request.Country;
        if (request.AvatarUrl != null) profile.AvatarUrl = request.AvatarUrl;
        if (request.DateOfBirth.HasValue) profile.DateOfBirth = request.DateOfBirth;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var authContext = new Auth.Data.AuthDbContext(
            new DbContextOptionsBuilder<Auth.Data.AuthDbContext>()
                .UseNpgsql(_context.Database.GetConnectionString()!)
                .Options);

        var user = await authContext.Users.FindAsync(userId);
        if (user == null) return false;

        var currentHash = HashPassword(request.CurrentPassword);
        if (user.PasswordHash != currentHash) return false;

        user.PasswordHash = HashPassword(request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await authContext.SaveChangesAsync();
        return true;
    }

    public async Task<UserSession?> CreateSessionAsync(Guid userId, string refreshToken, string? deviceInfo, string? ipAddress)
    {
        var session = new UserSession
        {
            UserId = userId,
            RefreshToken = refreshToken,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<bool> RevokeSessionAsync(string refreshToken)
    {
        var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
        if (session == null) return false;
        session.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeAllSessionsAsync(Guid userId)
    {
        var sessions = await _context.UserSessions.Where(s => s.UserId == userId && !s.IsRevoked).ToListAsync();
        foreach (var session in sessions) session.IsRevoked = true;
        await _context.SaveChangesAsync();
        return sessions.Any();
    }

    public async Task CleanupExpiredSessionsAsync()
    {
        var expiredSessions = await _context.UserSessions
            .Where(s => s.ExpiresAt < DateTime.UtcNow || s.IsRevoked).ToListAsync();
        _context.UserSessions.RemoveRange(expiredSessions);
        await _context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
