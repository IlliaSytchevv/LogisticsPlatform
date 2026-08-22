using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(x => x.CustomerName).HasMaxLength(256);
        builder.Property(x => x.PrimaryReference).HasMaxLength(64);
        builder.Property(x => x.Phone).HasMaxLength(64);
        builder.Property(x => x.TrailerType).HasMaxLength(64);
        builder.Property(x => x.TruckNumber).HasMaxLength(64);
        builder.Property(x => x.TrailerNumber).HasMaxLength(64);
        builder.Property(x => x.DockCode).HasMaxLength(32);
        builder.Property(x => x.DockBay).HasMaxLength(32);
        builder.Property(x => x.WarehouseNote).HasMaxLength(2000);
        builder.Property(x => x.StockStatusLabel).HasMaxLength(64);
        builder.Property(x => x.LoadingStatusLabel).HasMaxLength(64);
        builder.Property(x => x.ServicesCsv).HasMaxLength(512);
        builder.Property(x => x.QuantityUnitLabel).HasMaxLength(64);
        builder.Property(x => x.DockStatusLabel).HasMaxLength(128);

        builder.HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
