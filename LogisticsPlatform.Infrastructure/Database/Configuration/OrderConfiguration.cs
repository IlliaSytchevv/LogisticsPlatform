using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasOne(o => o.Hub)
            .WithMany(h => h.Orders)
            .HasForeignKey(o => o.HubId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(o => o.CreatedByUser)
            .WithMany()
            .HasForeignKey(o => o.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(o => o.Carrier)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CarrierId);

        builder.OwnsOne(o => o.NextAction, nextAction =>
        {
            nextAction.Property(x => x.AwaitingClientAction).HasColumnName("AwaitingClientAction");
            nextAction.Property(x => x.NextActionKind).HasColumnName("NextActionKind");
            nextAction.Property(x => x.NextActionLabel).HasColumnName("NextActionLabel");
            nextAction.Property(x => x.NextActionDueAt).HasColumnName("NextActionDueAt");
            nextAction.Property(x => x.NextActionAmountCents).HasColumnName("NextActionAmountCents");
            nextAction.Property(x => x.NextActionDocumentNumber).HasColumnName("NextActionDocumentNumber");
        });

        builder.OwnsOne(o => o.Cabinet, cabinet =>
        {
            cabinet.Property(x => x.CustomerName).HasColumnName("CustomerName").HasMaxLength(256);
            cabinet.Property(x => x.PrimaryReference).HasColumnName("PrimaryReference").HasMaxLength(64);
            cabinet.Property(x => x.Phone).HasColumnName("Phone").HasMaxLength(64);
            cabinet.Property(x => x.TrailerType).HasColumnName("TrailerType").HasMaxLength(64);
            cabinet.Property(x => x.TruckNumber).HasColumnName("TruckNumber").HasMaxLength(64);
            cabinet.Property(x => x.TrailerNumber).HasColumnName("TrailerNumber").HasMaxLength(64);
            cabinet.Property(x => x.ServicesCsv).HasColumnName("ServicesCsv").HasMaxLength(512);
            cabinet.Property(x => x.QuantityUnitLabel).HasColumnName("QuantityUnitLabel").HasMaxLength(64);
            cabinet.Property(x => x.StockStatusLabel).HasColumnName("StockStatusLabel").HasMaxLength(64);
            cabinet.Property(x => x.LoadingStatusLabel).HasColumnName("LoadingStatusLabel").HasMaxLength(64);
        });

        builder.OwnsOne(o => o.Dock, dock =>
        {
            dock.Property(x => x.DockCode).HasColumnName("DockCode").HasMaxLength(32);
            dock.Property(x => x.DockBay).HasColumnName("DockBay").HasMaxLength(32);
            dock.Property(x => x.DockAssignedAt).HasColumnName("DockAssignedAt");
            dock.Property(x => x.DockStatusLabel).HasColumnName("DockStatusLabel").HasMaxLength(128);
            dock.Property(x => x.AssignedToUserId).HasColumnName("AssignedToUserId");
            dock.Property(x => x.WarehouseNote).HasColumnName("WarehouseNote").HasMaxLength(2000);

            dock.HasOne(x => x.AssignedToUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
