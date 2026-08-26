namespace LogisticsPlatform.Application.DTO.Orders.Detail;

public sealed record OrderHubDockResponse(
    string Code,
    string? BayLabel,
    bool IsAssigned);
