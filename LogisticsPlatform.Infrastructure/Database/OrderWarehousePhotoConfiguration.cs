using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database;

public sealed class OrderWarehousePhotoConfiguration : IEntityTypeConfiguration<OrderWarehousePhoto>
{
    public void Configure(EntityTypeBuilder<OrderWarehousePhoto> builder)
    {
        builder.ToTable("OrderWarehousePhotos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Content).IsRequired();

        builder.HasOne(x => x.Order)
            .WithMany(o => o.WarehousePhotos)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
