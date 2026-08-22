using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.Dashboard.GetActiveOrders;

public sealed class GetDashboardActiveOrdersQueryValidator
    : AbstractValidator<GetDashboardActiveOrdersQuery>
{
    public GetDashboardActiveOrdersQueryValidator()
    {
        RuleFor(x => x.Take)
            .InclusiveBetween(1, 50);
    }
}
