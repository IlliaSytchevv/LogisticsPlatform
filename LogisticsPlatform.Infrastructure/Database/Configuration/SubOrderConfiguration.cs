using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class SubOrderConfiguration : IEntityTypeConfiguration<SubOrder>
{
    public void Configure(EntityTypeBuilder<SubOrder> builder)
    {
        builder.ToTable("SubOrders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasOne(x => x.Order)
            .WithMany(o => o.SubOrders)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
    }
}
