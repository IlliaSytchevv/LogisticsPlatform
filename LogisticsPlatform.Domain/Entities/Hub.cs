namespace LogisticsPlatform.Domain.Entities;

public class Hub
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? RegionCode { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<HubDock> Docks { get; set; } = new List<HubDock>();
}