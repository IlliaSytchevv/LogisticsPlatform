using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;

    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }

    public Guid HubId { get; set; }
    public Hub Hub { get; set; } = null!;

    public DateTimeOffset ScheduledAt { get; set; }

    public string DestinationCity { get; set; } = null!;
    public string DestinationRegion { get; set; } = null!;
    public string? DestinationNote { get; set; }

    public Guid CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public int? DeclaredQty { get; set; }
    public int? ActualQty { get; set; }

    public int? TrailersConsolidated { get; set; }

    public bool AwaitingClientAction { get; set; }
    public bool HasAlert { get; set; }
    public string? AlertReason { get; set; }

    public NextActionKind? NextActionKind { get; set; }
    public string? NextActionLabel { get; set; }
    public DateTimeOffset? NextActionDueAt { get; set; }
    public long? NextActionAmountCents { get; set; }
    public string? NextActionDocumentNumber { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public long SpendCents { get; set; }

    // Detail / cabinet fields
    public string? CustomerName { get; set; }
    public string? PrimaryReference { get; set; }
    public string? Phone { get; set; }
    public string? TrailerType { get; set; }
    public string? TruckNumber { get; set; }
    public string? TrailerNumber { get; set; }
    public string? DockCode { get; set; }
    public string? DockBay { get; set; }
    public DateTimeOffset? DockAssignedAt { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }
    public string? WarehouseNote { get; set; }
    public string? StockStatusLabel { get; set; }
    public string? LoadingStatusLabel { get; set; }
    public string? ServicesCsv { get; set; }
    public string? QuantityUnitLabel { get; set; }
    public string? DockStatusLabel { get; set; }

    public ICollection<OrderQuantityLine> QuantityLines { get; set; } = new List<OrderQuantityLine>();
    public ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
    public ICollection<OrderOperation> Operations { get; set; } = new List<OrderOperation>();
    public ICollection<OrderSupply> Supplies { get; set; } = new List<OrderSupply>();
    public ICollection<OrderWarehousePhoto> WarehousePhotos { get; set; } = new List<OrderWarehousePhoto>();
}
