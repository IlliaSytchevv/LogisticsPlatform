using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class OrderCommentConfiguration : IEntityTypeConfiguration<OrderComment>
{
    public void Configure(EntityTypeBuilder<OrderComment> builder)
    {
        builder.ToTable("OrderComments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.AuthorName).HasMaxLength(128);

        builder.HasOne(x => x.Order)
            .WithMany(o => o.Comments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => new { x.OrderId, x.CreatedAt })
            .HasDatabaseName("IX_OrderComments_OrderId_CreatedAt");
    }
}
