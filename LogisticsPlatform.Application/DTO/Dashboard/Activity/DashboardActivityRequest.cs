using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.DTO.Dashboard.Activity;

public sealed record DashboardActivityRequest(ActivityPeriod Period = ActivityPeriod.Month);
