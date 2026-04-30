using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.User.Models;

[Table("user_sessions")]
public class UserSession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("device_info")]
    public string? DeviceInfo { get; set; }

    [MaxLength(50)]
    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("is_revoked")]
    public bool IsRevoked { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Auth.Models.User? User { get; set; }
}
