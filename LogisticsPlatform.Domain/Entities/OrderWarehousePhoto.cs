namespace LogisticsPlatform.Domain.Entities;

public class OrderWarehousePhoto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Url { get; set; } = null!;
    public int SortOrder { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
