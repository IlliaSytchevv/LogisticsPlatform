using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Dashboard.Activity;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActivity;

public sealed record GetDashboardActivityQuery(ActivityPeriod Period)
    : IQuery<Result<DashboardActivityResponse>>;
