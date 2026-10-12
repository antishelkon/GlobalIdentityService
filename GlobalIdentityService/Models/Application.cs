using System;
using System.Collections.Generic;

namespace GlobalIdentityService.Models;

public partial class Application
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public bool Enabled { get; set; }

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<SecurityRoleAudit> SecurityRoleAudits { get; set; } = new List<SecurityRoleAudit>();

    public virtual ICollection<SecurityUserAudit> SecurityUserAudits { get; set; } = new List<SecurityUserAudit>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
