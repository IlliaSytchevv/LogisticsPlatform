namespace LogisticsPlatform.Domain.Entities;

public class OrderSupply
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Quantity { get; set; }
    public long UnitPriceCents { get; set; }
    public long LineTotalCents { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
