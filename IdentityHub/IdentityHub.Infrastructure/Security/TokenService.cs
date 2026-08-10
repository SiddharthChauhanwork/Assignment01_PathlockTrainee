
using IdentityHub.Application.DTOs.Authentication;
using IdentityHub.Application.Interfaces.Repositories;
using IdentityHub.Application.Interfaces.Services;
using IdentityHub.Domain.Entities;
using IdentityHub.Domain.Exceptions;
using System.Security.Cryptography;

namespace IdentityHub.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IClientRepository _clientRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly ISecretHasher _secretHasher;
    private readonly IRefreshTokenHasher _refreshTokenHasher;

    public TokenService(
        IClientRepository clientRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAccessTokenRepository accessTokenRepository,
        ISecretHasher secretHasher,
        IRefreshTokenHasher refreshTokenHasher)
    {
        _clientRepository = clientRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessTokenRepository = accessTokenRepository;
        _secretHasher = secretHasher;
        _refreshTokenHasher = refreshTokenHasher;
    }

    public async Task<TokenResponseDto> GenerateTokenAsync(
        TokenRequestDto request)
    {
        var client = await _clientRepository
            .GetByClientIdAsync(request.ClientId);

        if (client == null)
        {
            client = new ApplicationClient
            {
                ClientId = request.ClientId,

                SecretHash = _secretHasher
           .HashSecret(request.ClientSecret),

                CompanyName = request.ClientId,

                PrimaryDomain = "development.local",

                IsActive = true
            };

            await _clientRepository.AddAsync(client);
        }

        if (!client.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Client is inactive.");
        }

        var secretValid = _secretHasher.VerifySecret(
            request.ClientSecret,
            client.SecretHash);

        if (!secretValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid client credentials.");
        }

        var referenceToken = GenerateSecureToken();
        var refreshToken = GenerateSecureToken();

        var accessTokenEntity = new AccessToken
        {
            ApplicationClientId = client.Id,
            Email = request.Email,

            TokenHash = _refreshTokenHasher
                .HashToken(referenceToken),

            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        var refreshTokenEntity = new RefreshToken
        {
            ApplicationClientId = client.Id,
            Email = request.Email,

            TokenHash = _refreshTokenHasher
                .HashToken(refreshToken),

            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _accessTokenRepository
            .AddAsync(accessTokenEntity);

        await _refreshTokenRepository
            .AddAsync(refreshTokenEntity);

        return new TokenResponseDto
        {
            AccessToken = referenceToken,
            RefreshToken = refreshToken,
            TokenType = "Bearer",
            ExpiresIn = 900
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto request)
    {
        var tokenHash = _refreshTokenHasher
            .HashToken(request.RefreshToken);

        var storedToken = await _refreshTokenRepository
            .GetByTokenHashAsync(tokenHash);

        if (storedToken == null)
        {
            throw new RefreshTokenInvalidException();
        }

        if (!storedToken.IsActive)
        {
            throw new RefreshTokenInvalidException();
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevocationReason = "Token rotated.";

        var newReferenceToken = GenerateSecureToken();
        var newRefreshToken = GenerateSecureToken();

        var newAccessTokenEntity = new AccessToken
        {
            ApplicationClientId = storedToken.ApplicationClientId,
            Email = storedToken.Email,
            TokenHash = _refreshTokenHasher
                .HashToken(newReferenceToken),

            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        var newRefreshTokenEntity = new RefreshToken
        {
            ApplicationClientId = storedToken.ApplicationClientId,
            Email = storedToken.Email,
            TokenHash = _refreshTokenHasher
                .HashToken(newRefreshToken),

            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        storedToken.ReplacedByTokenHash =
            newRefreshTokenEntity.TokenHash;

        await _refreshTokenRepository
            .UpdateAsync(storedToken);

        await _accessTokenRepository
            .AddAsync(newAccessTokenEntity);

        await _refreshTokenRepository
            .AddAsync(newRefreshTokenEntity);

        return new TokenResponseDto
        {
            AccessToken = newReferenceToken,
            RefreshToken = newRefreshToken,
            TokenType = "Bearer",
            ExpiresIn = 900
        };
    
    }

    public async Task<bool> ValidateAccessTokenAsync(
        string accessToken)
    {
        var tokenHash = _refreshTokenHasher
            .HashToken(accessToken);

        var storedToken = await _accessTokenRepository
            .GetByTokenHashAsync(tokenHash);

        if (storedToken == null)
        {
            return false;
        }

        if (!storedToken.IsActive)
        {
            return false;
        }

        if (!storedToken.ApplicationClient.IsActive)
        {
            return false;
        }

        return true;
    }

    public async Task RevokeAccessTokenAsync(
        string accessToken)
    {
        var tokenHash = _refreshTokenHasher
            .HashToken(accessToken);

        var storedToken = await _accessTokenRepository
            .GetByTokenHashAsync(tokenHash);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid access token.");
        }

        if (storedToken.IsRevoked)
        {
            throw new UnauthorizedAccessException(
                "Access token is already revoked.");
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _accessTokenRepository
            .UpdateAsync(storedToken);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes);
    }
}
