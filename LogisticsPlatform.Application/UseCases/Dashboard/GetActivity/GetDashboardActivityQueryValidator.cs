using FluentValidation;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActivity;

public sealed class GetDashboardActivityQueryValidator : AbstractValidator<GetDashboardActivityQuery>
{
    public GetDashboardActivityQueryValidator()
    {
        RuleFor(x => x.Period)
            .IsInEnum()
            .Must(p => Enum.IsDefined(typeof(ActivityPeriod), p));
    }
}
