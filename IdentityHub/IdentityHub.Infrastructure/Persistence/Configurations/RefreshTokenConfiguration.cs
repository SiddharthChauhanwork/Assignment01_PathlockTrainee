using IdentityHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityHub.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.TokenHash)
               .IsRequired();

        builder.Property(rt => rt.ExpiresAt)
               .IsRequired();

        builder.Property(rt => rt.RevocationReason)
               .HasMaxLength(250);

        builder.Property(rt => rt.ReplacedByTokenHash)
               .HasMaxLength(500);
        builder.Property(x => x.Email)
    .IsRequired()
    .HasMaxLength(255);

        builder.HasOne(rt => rt.ApplicationClient)
               .WithMany()
               .HasForeignKey(rt => rt.ApplicationClientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}