using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Exceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException()
            : base("Email is already registered.")
        {
        }
    }
}
