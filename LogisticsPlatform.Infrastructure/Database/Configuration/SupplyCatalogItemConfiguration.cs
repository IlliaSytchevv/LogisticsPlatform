using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class SupplyCatalogItemConfiguration : IEntityTypeConfiguration<SupplyCatalogItem>
{
    public void Configure(EntityTypeBuilder<SupplyCatalogItem> builder)
    {
        builder.ToTable("SupplyCatalogItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(64).IsRequired();
        builder.Property(x => x.MarginSplitPercent).HasPrecision(5, 2);

        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.SortOrder })
            .HasDatabaseName("IX_SupplyCatalogItems_IsActive_SortOrder");
    }
}
