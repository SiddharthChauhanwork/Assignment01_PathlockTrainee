using IdentityHub.Application.Interfaces.Repositories;
using IdentityHub.Domain.Entities;
using IdentityHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityHub.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationClient?> GetByClientIdAsync(string clientId)
    {
        return await _context.ApplicationClients
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task AddAsync(ApplicationClient client)
    {
        await _context.ApplicationClients.AddAsync(client);
        await _context.SaveChangesAsync();
    }
}