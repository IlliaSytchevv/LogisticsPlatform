namespace LogisticsPlatform.Domain.DTO.Orders.TabCounts;

public sealed record OrdersTabCountsResponse(
    int All,
    int CrossDock,
    int Consolidation,
    int Alerts,
    int Drafts);
