using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Exceptions
{
    public class RefreshTokenInvalidException : Exception { 
        public RefreshTokenInvalidException() 
            : base("Refresh token is invalid, expired, or revoked.") 
        {
        }
    }
}
