using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Entities;

public class OrderOperation
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public OrderOperationType Type { get; set; }
    public string? Trailer { get; set; }
    public int Quantity { get; set; }
    public PalletUnit Unit { get; set; }
    public string? UnitLabel { get; set; }
    public DateTimeOffset AppliedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
