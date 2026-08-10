using IdentityHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityHub.Infrastructure.Security;

public class SecretHasher : ISecretHasher
{
    private readonly PasswordHasher<object> _hasher;

    public SecretHasher()
    {
        _hasher = new PasswordHasher<object>();
    }

    public string HashSecret(string secret)
    {
        return _hasher.HashPassword(null!, secret);
    }

    public bool VerifySecret(string secret, string hash)
    {
        var result = _hasher.VerifyHashedPassword(
            null!,
            hash,
            secret);

        return result == PasswordVerificationResult.Success;
    }
}