using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Tenant.Models;

[Table("tenants")]
public class Tenant
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(200)]
    [Column("domain")]
    public string? Domain { get; set; }

    [MaxLength(100)]
    [Column("industry")]
    public string? Industry { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("max_users")]
    public int MaxUsers { get; set; } = 100;

    [Column("settings")]
    public string? Settings { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("suspended_at")]
    public DateTime? SuspendedAt { get; set; }

    public virtual ICollection<Auth.Models.User> Users { get; set; } = new List<Auth.Models.User>();
    public virtual ICollection<Auth.Models.Role> Roles { get; set; } = new List<Auth.Models.Role>();
}
