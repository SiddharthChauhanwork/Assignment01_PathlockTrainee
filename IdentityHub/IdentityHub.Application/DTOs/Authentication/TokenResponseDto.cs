using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.DTOs.Authentication;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;


    public string TokenType { get; set; } = "Bearer";

    public int ExpiresIn { get; set; }
}