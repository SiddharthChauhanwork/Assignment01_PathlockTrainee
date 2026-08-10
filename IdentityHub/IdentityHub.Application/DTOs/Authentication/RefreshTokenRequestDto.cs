using System;
using System.Collections.Generic;
using System.Text;


namespace IdentityHub.Application.DTOs.Authentication;

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
