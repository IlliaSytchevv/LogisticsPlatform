namespace LogisticsPlatform.Application.DTO.Dashboard.Metrics;

public sealed record NeedAttentionMetricResponse(
    int Total,
    int AwaitingAction,
    int Alerts,
    IReadOnlyList<AlertSampleResponse> AlertSamples);
