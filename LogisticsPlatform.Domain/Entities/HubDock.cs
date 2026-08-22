namespace LogisticsPlatform.Domain.Entities;

public class HubDock
{
    public Guid Id { get; set; }
    public Guid HubId { get; set; }
    public Hub Hub { get; set; } = null!;

    public string Code { get; set; } = null!;
    public string? BayLabel { get; set; }
    public int SortOrder { get; set; }
}
