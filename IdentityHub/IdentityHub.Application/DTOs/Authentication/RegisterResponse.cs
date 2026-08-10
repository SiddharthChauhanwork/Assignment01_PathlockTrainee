using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.DTOs.Authentication
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = default!;

       
    }
}
