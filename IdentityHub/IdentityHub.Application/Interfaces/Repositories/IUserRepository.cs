using IdentityHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(
            Guid applicationClientId,
            string email);

        Task AddAsync(ApplicationUser user);
    }
}
