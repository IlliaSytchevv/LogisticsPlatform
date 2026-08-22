using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.DTO.Dashboard.Activity;

public sealed record DashboardActivityRequest(ActivityPeriod Period = ActivityPeriod.Month);
