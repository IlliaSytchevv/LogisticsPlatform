using FluentValidation;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.GetTabCounts;

public sealed class GetOrdersTabCountsQueryValidator : AbstractValidator<GetOrdersTabCountsQuery>
{
    public GetOrdersTabCountsQueryValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => s is null || Enum.IsDefined(typeof(OrderStatus), s.Value))
            .WithMessage("Invalid status.");
        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}
