using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.DTOs.Authentication;

public class TokenRequestDto
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}