using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {


        public Guid ApplicationClientId { get; set; }
        public ApplicationClient ApplicationClient { get; set; } = default!;
        public string Email { get; set; } = string.Empty;
        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? RevocationReason { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsRevoked => RevokedAt != null;

        public bool IsActive => !IsExpired && !IsRevoked;
    }

}
