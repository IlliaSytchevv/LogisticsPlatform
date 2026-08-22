namespace LogisticsPlatform.Domain.DTO.Dashboard.Activity;

public sealed record ActivitySeriesPointResponse(
    string Label,
    int Value,
    long ValueCents);
