namespace Modules.Tenant.Models;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Domain { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; }
    public int MaxUsers { get; set; }
    public string? Settings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
}

public class CreateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Domain { get; set; }
    public string? Industry { get; set; }
    public int MaxUsers { get; set; } = 100;
    public string? Settings { get; set; }
}

public class UpdateTenantRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Domain { get; set; }
    public string? Industry { get; set; }
    public int? MaxUsers { get; set; }
    public string? Settings { get; set; }
}
