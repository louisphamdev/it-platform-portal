namespace Modules.Permission.Models;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RolePermissionDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
}

public class AssignPermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
    public Guid GrantedBy { get; set; }
}
