using System;
using System.Collections.Generic;

namespace GlobalIdentityService.Models;

public partial class Role
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();

    public virtual ICollection<SecurityRoleAudit> SecurityRoleAudits { get; set; } = new List<SecurityRoleAudit>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
