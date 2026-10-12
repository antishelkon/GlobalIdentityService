using System;
using System.Collections.Generic;

namespace GlobalIdentityService.Models;

public partial class RolePrivilege
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long PrivilegeId { get; set; }

    public virtual Privilege Privilege { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
