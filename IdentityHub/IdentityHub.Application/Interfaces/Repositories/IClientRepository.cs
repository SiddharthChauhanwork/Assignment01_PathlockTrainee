using IdentityHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
     
        Task<ApplicationClient?> GetByClientIdAsync(string clientId);
        Task AddAsync(ApplicationClient client);
    }
}
