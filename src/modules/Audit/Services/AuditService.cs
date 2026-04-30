using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modules.Audit.Data;
using Modules.Audit.Models;

namespace Modules.Audit.Services;

public interface IAuditService
{
    Task LogAsync(AuditLogDto logDto);
    Task<List<AuditLogDto>> QueryAsync(AuditQueryRequest request);
    Task<AuditLogDto?> GetByIdAsync(Guid id);
    Task<int> GetTotalCountAsync(AuditQueryRequest request);
}

public class AuditService : IAuditService
{
    private readonly AuditDbContext _context;

    public AuditService(AuditDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(AuditLogDto logDto)
    {
        var log = new AuditLog
        {
            Action = logDto.Action,
            EntityType = logDto.EntityType,
            EntityId = logDto.EntityId,
            TenantId = logDto.TenantId,
            UserId = logDto.UserId,
            UserName = logDto.UserName,
            UserRole = logDto.UserRole,
            IpAddress = logDto.IpAddress,
            UserAgent = logDto.UserAgent,
            RequestData = logDto.RequestData,
            ResponseData = logDto.ResponseData,
            StatusCode = logDto.StatusCode,
            ErrorMessage = logDto.ErrorMessage,
            DurationMs = logDto.DurationMs,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AuditLogDto>> QueryAsync(AuditQueryRequest request)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(l => l.Action == request.Action);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(l => l.EntityType == request.EntityType);

        if (request.TenantId.HasValue)
            query = query.Where(l => l.TenantId == request.TenantId);

        if (request.UserId.HasValue)
            query = query.Where(l => l.UserId == request.UserId);

        if (request.FromDate.HasValue)
            query = query.Where(l => l.CreatedAt >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(l => l.CreatedAt <= request.ToDate);

        return query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id)
    {
        var log = await _context.AuditLogs.FindAsync(id);
        return log == null ? null : MapToDto(log);
    }

    public async Task<int> GetTotalCountAsync(AuditQueryRequest request)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(l => l.Action == request.Action);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(l => l.EntityType == request.EntityType);

        if (request.TenantId.HasValue)
            query = query.Where(l => l.TenantId == request.TenantId);

        if (request.UserId.HasValue)
            query = query.Where(l => l.UserId == request.UserId);

        if (request.FromDate.HasValue)
            query = query.Where(l => l.CreatedAt >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(l => l.CreatedAt <= request.ToDate);

        return query.Count();
    }

    private static AuditLogDto MapToDto(AuditLog log) => new()
    {
        Id = log.Id,
        Action = log.Action,
        EntityType = log.EntityType,
        EntityId = log.EntityId,
        TenantId = log.TenantId,
        UserId = log.UserId,
        UserName = log.UserName,
        UserRole = log.UserRole,
        IpAddress = log.IpAddress,
        UserAgent = log.UserAgent,
        RequestData = log.RequestData,
        ResponseData = log.ResponseData,
        StatusCode = log.StatusCode,
        ErrorMessage = log.ErrorMessage,
        DurationMs = log.DurationMs,
        CreatedAt = log.CreatedAt
    };
}
