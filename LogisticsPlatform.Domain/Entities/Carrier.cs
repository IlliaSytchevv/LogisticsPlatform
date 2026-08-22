namespace LogisticsPlatform.Domain.Entities;

public class Carrier
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public Guid? DriverUserId { get; set; }
    public ApplicationUser? DriverUser { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}