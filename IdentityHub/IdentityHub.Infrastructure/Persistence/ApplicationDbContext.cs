using IdentityHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityHub.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationClient> ApplicationClients => Set<ApplicationClient>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Kept for future expansion.
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<AccessToken> AccessTokens => Set<AccessToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly); //IT will search assembly and find all classes that implement IEntityTypeConfiguration<T>
    }
}