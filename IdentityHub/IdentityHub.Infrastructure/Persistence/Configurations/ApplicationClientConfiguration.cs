using IdentityHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityHub.Infrastructure.Persistence.Configurations;

public class ApplicationClientConfiguration : IEntityTypeConfiguration<ApplicationClient>
{
    public void Configure(EntityTypeBuilder<ApplicationClient> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ClientId)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(c => c.ClientId)
               .IsUnique();

        builder.Property(c => c.SecretHash)
               .IsRequired();

        builder.Property(c => c.CompanyName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(c => c.PrimaryDomain)
               .IsRequired()
               .HasMaxLength(255);

    }
}