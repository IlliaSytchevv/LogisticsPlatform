using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database;

public sealed class OrderOperationCommentConfiguration : IEntityTypeConfiguration<OrderOperationComment>
{
    public void Configure(EntityTypeBuilder<OrderOperationComment> builder)
    {
        builder.ToTable("OrderOperationComments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.AuthorName).HasMaxLength(128);

        builder.HasOne(x => x.Operation)
            .WithMany(o => o.Comments)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OperationId);
    }
}
