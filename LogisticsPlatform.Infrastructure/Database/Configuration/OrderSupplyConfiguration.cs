using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderSupplyConfiguration : IEntityTypeConfiguration<OrderSupply>
{
    public void Configure(EntityTypeBuilder<OrderSupply> builder)
    {
        builder.ToTable("OrderSupplies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(64).IsRequired();

        builder.HasOne(x => x.Order)
            .WithMany(o => o.Supplies)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => new { x.OrderId, x.IsDeleted })
            .HasDatabaseName("IX_OrderSupplies_OrderId_IsDeleted");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
