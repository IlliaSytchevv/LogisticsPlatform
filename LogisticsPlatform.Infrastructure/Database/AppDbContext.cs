using LogisticsPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Hub> Hubs { get; set; }
    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderQuantityLine> OrderQuantityLines { get; set; }
    public DbSet<SubOrder> SubOrders { get; set; }
    public DbSet<OrderOperation> OrderOperations { get; set; }
    public DbSet<OrderOperationComment> OrderOperationComments { get; set; }
    public DbSet<OrderOperationPhoto> OrderOperationPhotos { get; set; }
    public DbSet<OrderSupply> OrderSupplies { get; set; }
    public DbSet<OrderWarehousePhoto> OrderWarehousePhotos { get; set; }
    public DbSet<OrderComment> OrderComments { get; set; }
    public DbSet<OrderTimelineEntry> OrderTimelineEntries { get; set; }
    public DbSet<HubDock> HubDocks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
