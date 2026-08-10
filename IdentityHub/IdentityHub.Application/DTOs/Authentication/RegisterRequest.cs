using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.DTOs.Authentication
{
    public class RegisterRequest
    {
        public Guid ApplicationClientId { get; set; }

        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;
    }
}
