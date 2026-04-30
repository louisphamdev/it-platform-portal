namespace Modules.Audit.Models;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserRole { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public long? DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditQueryRequest
{
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
