using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException()
            : base("Application client was not found.")
        {
        }
    }
}
