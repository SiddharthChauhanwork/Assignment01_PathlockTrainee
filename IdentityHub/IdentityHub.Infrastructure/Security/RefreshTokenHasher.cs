using System.Security.Cryptography;
using System.Text;
using IdentityHub.Application.Interfaces.Services;

namespace IdentityHub.Infrastructure.Security;

public class RefreshTokenHasher : IRefreshTokenHasher
{
    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }
}
