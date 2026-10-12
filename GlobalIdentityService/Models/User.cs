using System;
using System.Collections.Generic;

namespace GlobalIdentityService.Models;

public partial class User
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string PasswordHash { get; set; } = null!;

    public long? AvatarId { get; set; }

    public string Status { get; set; } = null!;

    public bool EmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<SecurityRoleAudit> SecurityRoleAudits { get; set; } = new List<SecurityRoleAudit>();

    public virtual ICollection<SecurityUserAudit> SecurityUserAuditActorUsers { get; set; } = new List<SecurityUserAudit>();

    public virtual ICollection<SecurityUserAudit> SecurityUserAuditTargetUsers { get; set; } = new List<SecurityUserAudit>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
