using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Entities;

public class OrderQuantityLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public PalletUnit Unit { get; set; }
    public int Count { get; set; }
}