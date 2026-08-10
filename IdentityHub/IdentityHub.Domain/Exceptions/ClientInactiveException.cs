using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Domain.Exceptions
{
    public class ClientInactiveException : Exception
    {
        public ClientInactiveException()
            : base("Application client is inactive.")
        {
        }
    }
}
