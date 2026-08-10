using IdentityHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

        Task AddAsync(RefreshToken refreshToken);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}
