namespace LogisticsPlatform.Domain.DTO.Dashboard.Metrics;

public sealed record AlertSampleResponse(
    string OrderNumber,
    string Reason);
