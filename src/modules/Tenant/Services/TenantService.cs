using Microsoft.EntityFrameworkCore;
using Modules.Tenant.Data;
using Modules.Tenant.Models;

namespace Modules.Tenant.Services;

public interface ITenantService
{
    Task<List<TenantDto>> GetAllTenantsAsync();
    Task<TenantDto?> GetTenantByIdAsync(Guid id);
    Task<TenantDto?> GetTenantByCodeAsync(string code);
    Task<TenantDto> CreateTenantAsync(CreateTenantRequest request);
    Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantRequest request);
    Task<bool> SuspendTenantAsync(Guid id);
    Task<bool> ActivateTenantAsync(Guid id);
    Task<int> GetUserCountAsync(Guid tenantId);
}

public class TenantService : ITenantService
{
    private readonly TenantDbContext _context;

    public TenantService(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantDto>> GetAllTenantsAsync()
    {
        return _context.Tenants.Select(MapToDto).ToList();
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto?> GetTenantByCodeAsync(string code)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Code == code);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto> CreateTenantAsync(CreateTenantRequest request)
    {
        var tenant = new Tenant
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Domain = request.Domain,
            Industry = request.Industry,
            MaxUsers = request.MaxUsers,
            Settings = request.Settings,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return MapToDto(tenant);
    }

    public async Task<TenantDto?> UpdateTenantAsync(Guid id, UpdateTenantRequest request)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return null;

        if (request.Name != null) tenant.Name = request.Name;
        if (request.Description != null) tenant.Description = request.Description;
        if (request.Domain != null) tenant.Domain = request.Domain;
        if (request.Industry != null) tenant.Industry = request.Industry;
        if (request.MaxUsers.HasValue) tenant.MaxUsers = request.MaxUsers.Value;
        if (request.Settings != null) tenant.Settings = request.Settings;

        tenant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(tenant);
    }

    public async Task<bool> SuspendTenantAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return false;
        tenant.IsActive = false;
        tenant.SuspendedAt = DateTime.UtcNow;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateTenantAsync(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return false;
        tenant.IsActive = true;
        tenant.SuspendedAt = null;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUserCountAsync(Guid tenantId)
    {
        return await _context.Tenants.Where(t => t.Id == tenantId).SelectMany(t => t.Users).CountAsync();
    }

    private static TenantDto MapToDto(Tenant tenant) => new()
    {
        Id = tenant.Id,
        Name = tenant.Name,
        Code = tenant.Code,
        Description = tenant.Description,
        Domain = tenant.Domain,
        Industry = tenant.Industry,
        IsActive = tenant.IsActive,
        MaxUsers = tenant.MaxUsers,
        Settings = tenant.Settings,
        CreatedAt = tenant.CreatedAt,
        SuspendedAt = tenant.SuspendedAt
    };
}
