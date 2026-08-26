namespace LogisticsPlatform.ApiRoutes;

internal static class ApiRoutes
{
    private const string V1 = "api/v1";
    
    public const string Auth = $"{V1}/auth";
    public const string Dashboard = $"{V1}/dashboard";
    public const string Notifications = $"{V1}/notifications";
    public const string Supplies = $"{V1}/supplies";
    public const string Seed = $"{V1}/seed";
    public const string Orders = $"{V1}/orders";
    public const string Order = $"{Orders}/{{orderId:guid}}";
    public const string Payments = $"{V1}/payments";
}