using System;
using System.Collections.Generic;
using System.Net;

namespace GlobalIdentityService.Models;

public partial class SecurityRoleAudit
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long? ActorUserId { get; set; }

    public string EventType { get; set; } = null!;

    public IPAddress? Ip { get; set; }

    public long ApplicationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Metadata { get; set; } = null!;

    public virtual User? ActorUser { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
