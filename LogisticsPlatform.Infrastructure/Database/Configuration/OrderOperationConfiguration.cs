using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderOperationConfiguration : IEntityTypeConfiguration<OrderOperation>
{
    public void Configure(EntityTypeBuilder<OrderOperation> builder)
    {
        builder.ToTable("OrderOperations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Trailer).HasMaxLength(64);
        builder.Property(x => x.Unit).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.UnitLabel).HasMaxLength(64);

        builder.HasOne(x => x.Order)
            .WithMany(o => o.Operations)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
