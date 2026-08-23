using LogisticsPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsPlatform.Infrastructure.Database.Configuration;

public sealed class HubDockConfiguration : IEntityTypeConfiguration<HubDock>
{
    public void Configure(EntityTypeBuilder<HubDock> builder)
    {
        builder.ToTable("HubDocks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.BayLabel).HasMaxLength(32);

        builder.HasOne(x => x.Hub)
            .WithMany(h => h.Docks)
            .HasForeignKey(x => x.HubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.HubId, x.Code }).IsUnique();
    }
}
