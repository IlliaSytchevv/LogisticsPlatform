namespace LogisticsPlatform.Domain.Entities;

public class OrderCabinetDetail
{
    public string? CustomerName { get; set; }
    public string? PrimaryReference { get; set; }
    public string? Phone { get; set; }
    public string? TrailerType { get; set; }
    public string? TruckNumber { get; set; }
    public string? TrailerNumber { get; set; }
    public string? ServicesCsv { get; set; }
    public string? QuantityUnitLabel { get; set; }
    public string? StockStatusLabel { get; set; }
    public string? LoadingStatusLabel { get; set; }
}
