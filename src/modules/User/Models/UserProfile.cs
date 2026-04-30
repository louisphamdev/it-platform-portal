using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.User.Models;

[Table("user_profiles")]
public class UserProfile
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [MaxLength(100)]
    [Column("department")]
    public string? Department { get; set; }

    [MaxLength(100)]
    [Column("job_title")]
    public string? JobTitle { get; set; }

    [MaxLength(500)]
    [Column("address")]
    public string? Address { get; set; }

    [MaxLength(50)]
    [Column("city")]
    public string? City { get; set; }

    [MaxLength(50)]
    [Column("country")]
    public string? Country { get; set; }

    [MaxLength(500)]
    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Auth.Models.User? User { get; set; }
}
