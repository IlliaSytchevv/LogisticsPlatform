namespace LogisticsPlatform.Domain.Entities;

public class SubOrder
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string Number { get; set; } = null!;
    public string Reference { get; set; } = null!;
    public int PalletCount { get; set; }
    public bool HasMissingPhoto { get; set; }
    public int SortOrder { get; set; }
}