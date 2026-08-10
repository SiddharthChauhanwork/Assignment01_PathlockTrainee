using System;
using System.Collections.Generic;
using System.Text;
using IdentityHub.Application.Interfaces.Repositories;
using IdentityHub.Domain.Entities;
using IdentityHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityHub.Infrastructure.Repositories;

public class AccessTokenRepository : IAccessTokenRepository
{
    private readonly ApplicationDbContext _context;

    public AccessTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccessToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.AccessTokens
            .Include(at => at.ApplicationClient)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task AddAsync(AccessToken accessToken)
    {
        await _context.AccessTokens.AddAsync(accessToken);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AccessToken accessToken)
    {
        _context.AccessTokens.Update(accessToken);
        await _context.SaveChangesAsync();
    }
}
