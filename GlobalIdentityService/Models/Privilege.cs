using System;
using System.Collections.Generic;

namespace GlobalIdentityService.Models;

public partial class Privilege
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long ApplicationId { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
}
