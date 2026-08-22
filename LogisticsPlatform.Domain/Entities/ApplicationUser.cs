using LogisticsPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace LogisticsPlatform.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string ExternalId { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public string DisplayName { get; set; } = null!;
    public string Initials { get; set; } = null!;
    public UserRole Role { get; set; }
    public long BalanceCents { get; set; }
    public bool IsActive { get; set; } = true;
}
