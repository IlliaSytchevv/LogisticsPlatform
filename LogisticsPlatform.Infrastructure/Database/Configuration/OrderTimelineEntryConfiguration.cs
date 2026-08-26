using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderTimelineEntryConfiguration : IEntityTypeConfiguration<OrderTimelineEntry>
{
    public void Configure(EntityTypeBuilder<OrderTimelineEntry> builder)
    {
        builder.ToTable("OrderTimelineEntries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Kind).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Text).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.PreviousStatus);
        builder.Property(x => x.NewStatus);
        builder.Property(x => x.AuthorName).HasMaxLength(128);

        builder.HasOne(x => x.Order)
            .WithMany(o => o.TimelineEntries)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => new { x.OrderId, x.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_OrderTimelineEntries_OrderId_CreatedAt");
    }
}
