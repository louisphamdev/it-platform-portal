using Microsoft.EntityFrameworkCore;
using Modules.Permission.Data;
using Modules.Permission.Models;

namespace Modules.Permission.Services;

public interface IPermissionService
{
    Task<List<PermissionDto>> GetAllPermissionsAsync();
    Task<List<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId);
    Task<List<string>> GetPermissionCodesByUserAsync(Guid userId);
    Task<bool> AssignPermissionsAsync(AssignPermissionsRequest request);
    Task<bool> RemovePermissionAsync(Guid roleId, Guid permissionId);
    Task<PermissionDto> CreatePermissionAsync(PermissionDto dto);
}

public class PermissionService : IPermissionService
{
    private readonly PermissionDbContext _context;

    public PermissionService(PermissionDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionDto>> GetAllPermissionsAsync()
    {
        return _context.Permissions.Select(MapToDto).ToList();
    }

    public async Task<List<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId)
    {
        return _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => MapToDto(rp.Permission))
            .ToList();
    }

    public async Task<List<string>> GetPermissionCodesByUserAsync(Guid userId)
    {
        return _context.RolePermissions
            .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();
    }

    public async Task<bool> AssignPermissionsAsync(AssignPermissionsRequest request)
    {
        var existing = _context.RolePermissions
            .Where(rp => rp.RoleId == request.RoleId)
            .Select(rp => rp.PermissionId)
            .ToHashSet();

        var newPermissions = request.PermissionIds.Except(existing).ToList();

        foreach (var permId in newPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = permId,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = request.GrantedBy
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemovePermissionAsync(Guid roleId, Guid permissionId)
    {
        var rp = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);
        if (rp == null) return false;
        _context.RolePermissions.Remove(rp);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PermissionDto> CreatePermissionAsync(PermissionDto dto)
    {
        var perm = new Permission
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            Category = dto.Category,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Permissions.Add(perm);
        await _context.SaveChangesAsync();
        return MapToDto(perm);
    }

    private static PermissionDto MapToDto(Permission p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Code = p.Code,
        Description = p.Description,
        Category = p.Category,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}
