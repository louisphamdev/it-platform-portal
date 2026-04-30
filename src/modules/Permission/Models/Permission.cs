using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Permission.Models;

[Table("permissions")]
public class Permission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(50)]
    [Column("category")]
    public string? Category { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

[Table("role_permissions")]
public class RolePermission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("role_id")]
    public Guid RoleId { get; set; }

    [Column("permission_id")]
    public Guid PermissionId { get; set; }

    [Column("granted_at")]
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    [Column("granted_by")]
    public Guid? GrantedBy { get; set; }

    [ForeignKey(nameof(RoleId))]
    public virtual Auth.Models.Role Role { get; set; } = null!;

    [ForeignKey(nameof(PermissionId))]
    public virtual Permission Permission { get; set; } = null!;
}
