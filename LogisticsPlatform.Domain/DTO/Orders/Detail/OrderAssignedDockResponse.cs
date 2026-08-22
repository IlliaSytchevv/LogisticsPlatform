namespace LogisticsPlatform.Domain.DTO.Orders.Detail;

public sealed record OrderAssignedDockResponse(
    string HubName,
    string? DockCode,
    string? DockBay,
    string? TrailerNumber,
    DateTimeOffset? AssignedAt,
    string? StatusLabel,
    IReadOnlyList<OrderHubDockResponse> HubDocks);
