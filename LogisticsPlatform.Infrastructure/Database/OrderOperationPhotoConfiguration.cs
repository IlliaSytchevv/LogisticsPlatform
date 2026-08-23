using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database;

public sealed class OrderOperationPhotoConfiguration : IEntityTypeConfiguration<OrderOperationPhoto>
{
    public void Configure(EntityTypeBuilder<OrderOperationPhoto> builder)
    {
        builder.ToTable("OrderOperationPhotos");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Content).IsRequired();

        builder.HasOne(x => x.Operation)
            .WithMany(o => o.Photos)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OperationId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
