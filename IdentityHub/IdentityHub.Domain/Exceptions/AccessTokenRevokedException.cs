using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Exceptions
{
    public class AccessTokenRevokedException : Exception {
        public AccessTokenRevokedException() : base("Access token is already revoked.") 
        {
        }
    }
}
