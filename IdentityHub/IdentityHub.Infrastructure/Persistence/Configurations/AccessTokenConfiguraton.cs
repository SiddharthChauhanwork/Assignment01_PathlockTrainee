using System;
using System.Collections.Generic;
using System.Text;
using IdentityHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityHub.Infrastructure.Persistence.Configurations;

public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .IsRequired(false);

        builder.HasOne(x => x.ApplicationClient)
            .WithMany()
            .HasForeignKey(x => x.ApplicationClientId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Email)
    .IsRequired()
    .HasMaxLength(255);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();
    }
}
