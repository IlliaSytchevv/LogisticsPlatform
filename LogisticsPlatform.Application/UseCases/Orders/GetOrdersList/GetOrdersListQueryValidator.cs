using FluentValidation;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.GetOrdersList;

public sealed class GetOrdersListQueryValidator : AbstractValidator<GetOrdersListQuery>
{
    public GetOrdersListQueryValidator()
    {
        RuleFor(x => x.Tab)
            .IsInEnum();
        
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
        
        RuleFor(x => x.Status)
            .Must(s => s is null || Enum.IsDefined(typeof(OrderStatus), s.Value))
            .WithMessage("Invalid status.");
        
        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}
