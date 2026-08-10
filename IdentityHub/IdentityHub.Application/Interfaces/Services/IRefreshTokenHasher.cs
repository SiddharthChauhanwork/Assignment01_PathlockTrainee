using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Services;

public interface IRefreshTokenHasher
{
    string HashToken(string token);
}
