namespace LogisticsPlatform.Domain.Entities;

public class SupplyCatalogItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public long PlatformPriceCents { get; set; }
    public long WholesalePriceCents { get; set; }
    public decimal MarginSplitPercent { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}