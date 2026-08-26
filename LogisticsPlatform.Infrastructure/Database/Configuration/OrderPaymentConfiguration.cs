using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.ToTable("OrderPayments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Currency)
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(x => x.StripeSessionId)
            .HasMaxLength(256);

        builder.HasOne(x => x.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.StripeSessionId)
            .IsUnique()
            .HasFilter("\"StripeSessionId\" IS NOT NULL");
        builder.HasIndex(x => new { x.OrderId, x.Status })
            .HasDatabaseName("IX_OrderPayments_OrderId_Status");
    }
}
