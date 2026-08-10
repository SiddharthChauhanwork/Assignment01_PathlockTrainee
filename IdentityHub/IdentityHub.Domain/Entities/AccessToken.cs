using System;
using System.Collections.Generic;
using System.Text;


namespace IdentityHub.Domain.Entities;

public class AccessToken : BaseEntity
{
    public Guid ApplicationClientId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public ApplicationClient ApplicationClient { get; set; } = default!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;
}

