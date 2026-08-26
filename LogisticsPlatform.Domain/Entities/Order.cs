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

    public bool HasAlert { get; set; }
    public string? AlertReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public long SpendCents { get; set; }

    public OrderNextAction NextAction { get; set; } = new();
    public OrderCabinetDetail Cabinet { get; set; } = new();
    public OrderDockAssignment Dock { get; set; } = new();

    public ICollection<OrderQuantityLine> QuantityLines { get; set; } = new List<OrderQuantityLine>();
    public ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
    public ICollection<OrderOperation> Operations { get; set; } = new List<OrderOperation>();
    public ICollection<OrderSupply> Supplies { get; set; } = new List<OrderSupply>();
    public ICollection<OrderWarehousePhoto> WarehousePhotos { get; set; } = new List<OrderWarehousePhoto>();
    public ICollection<OrderComment> Comments { get; set; } = new List<OrderComment>();
    public ICollection<OrderTimelineEntry> TimelineEntries { get; set; } = new List<OrderTimelineEntry>();
    public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();
}