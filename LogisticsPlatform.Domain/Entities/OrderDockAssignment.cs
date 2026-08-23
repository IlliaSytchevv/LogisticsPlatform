namespace LogisticsPlatform.Domain.Entities;

public class OrderDockAssignment
{
    public string? DockCode { get; set; }
    public string? DockBay { get; set; }
    public DateTimeOffset? DockAssignedAt { get; set; }
    public string? DockStatusLabel { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }
    public string? WarehouseNote { get; set; }
}
