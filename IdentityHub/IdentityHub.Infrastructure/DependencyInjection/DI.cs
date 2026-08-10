using IdentityHub.Application.Interfaces.Repositories;
using IdentityHub.Application.Interfaces.Services;
using IdentityHub.Infrastructure.Persistence;
using IdentityHub.Infrastructure.Persistence.Repositories;
using IdentityHub.Infrastructure.Repositories;
using IdentityHub.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityHub.Infrastructure.DependencyInjection;

public static class DI
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IClientRepository, ClientRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();

        services.AddScoped<ISecretHasher, SecretHasher>();

        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}


