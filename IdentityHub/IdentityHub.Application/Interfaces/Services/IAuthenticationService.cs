
using IdentityHub.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDto> GenerateTokenAsync(TokenRequestDto request);

        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
