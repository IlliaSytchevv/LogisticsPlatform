using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderOperationPhotoConfiguration : IEntityTypeConfiguration<OrderOperationPhoto>
{
    public void Configure(EntityTypeBuilder<OrderOperationPhoto> builder)
    {
        builder.ToTable("OrderOperationPhotos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();

        builder.HasOne(x => x.Operation)
            .WithMany(o => o.Photos)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OperationId);
        builder.HasIndex(x => x.StorageKey)
            .IsUnique()
            .HasDatabaseName("IX_OrderOperationPhotos_StorageKey");
        builder.HasIndex(x => new { x.OperationId, x.IsDeleted })
            .HasDatabaseName("IX_OrderOperationPhotos_OperationId_IsDeleted");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
