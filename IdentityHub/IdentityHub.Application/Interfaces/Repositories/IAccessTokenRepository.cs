using System;
using System.Collections.Generic;
using System.Text;

using IdentityHub.Domain.Entities;

namespace IdentityHub.Application.Interfaces.Repositories;

public interface IAccessTokenRepository
{
    Task<AccessToken?> GetByTokenHashAsync(string tokenHash);

    Task AddAsync(AccessToken accessToken);

    Task UpdateAsync(AccessToken accessToken);
}

