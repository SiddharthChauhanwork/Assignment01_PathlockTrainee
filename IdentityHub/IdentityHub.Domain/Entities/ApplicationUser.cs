using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Entities
{
    public class ApplicationUser : BaseEntity
    {
        public Guid ApplicationClientId { get; set; }

        public string Email { get; set; } = default!;

        public string PasswordHash { get; set; } = default!;

        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public bool EmailConfirmed { get; set; } = false;
    }
}
