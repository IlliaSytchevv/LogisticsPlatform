using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.UserName)
            .HasMaxLength(256);

        builder.Property(u => u.Email)
            .HasMaxLength(256);

        builder.Property(u => u.ExternalId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(u => u.Initials)
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(u => u.BalanceCents)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.HasIndex(u => u.ExternalId)
            .IsUnique();

        builder.HasIndex(u => u.NormalizedUserName)
            .IsUnique()
            .HasFilter("\"NormalizedUserName\" IS NOT NULL");

        builder.HasIndex(u => u.NormalizedEmail);
    }
}
