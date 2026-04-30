using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Audit.Models;

[Table("audit_logs")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [Column("action")]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    [Column("entity_id")]
    public Guid? EntityId { get; set; }

    [Column("tenant_id")]
    public Guid? TenantId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [MaxLength(50)]
    [Column("user_name")]
    public string? UserName { get; set; }

    [MaxLength(50)]
    [Column("user_role")]
    public string? UserRole { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("request_data")]
    public string? RequestData { get; set; }

    [Column("response_data")]
    public string? ResponseData { get; set; }

    [Column("status_code")]
    public int? StatusCode { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("duration_ms")]
    public long? DurationMs { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
