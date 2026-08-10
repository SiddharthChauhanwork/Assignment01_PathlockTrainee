using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Services
{
    public interface ISecretHasher
    {
        string HashSecret(string secret);

        bool VerifySecret(string secret, string hash);
    }
}
