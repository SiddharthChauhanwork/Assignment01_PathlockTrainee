using IdentityHub.Application.DTOs.Authentication;
using IdentityHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken(
        [FromBody] TokenRequestDto request)
    {
        var response = await _tokenService
            .GenerateTokenAsync(request);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request)
    {
        var response = await _tokenService
            .RefreshTokenAsync(request);

        return Ok(response);
    }
   
    [HttpPost("validate")]
public async Task<IActionResult> ValidateAccessToken(
    [FromBody] string accessToken)
    {
        var isValid = await _tokenService
            .ValidateAccessTokenAsync(accessToken);

        if (!isValid)
        {
            return Unauthorized(new
            {
                message = "Invalid or inactive access token."
            });
        }

        return Ok(new
        {
            valid = true
        });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeAccessToken(
        [FromBody] string accessToken)
    {
        await _tokenService
            .RevokeAccessTokenAsync(accessToken);

        return Ok(new
        {
            message = "Access token revoked successfully."
        });
    }
}
