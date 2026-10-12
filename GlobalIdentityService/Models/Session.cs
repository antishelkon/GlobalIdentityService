using System;
using System.Collections.Generic;
using System.Net;

namespace GlobalIdentityService.Models;

public partial class Session
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string RefreshTokenHash { get; set; } = null!;

    public long ApplicationId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public IPAddress? Ip { get; set; }

    public string? UserAgent { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
