using FluentValidation;
using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Application.UseCases.Orders.ExportOrders;

public sealed class ExportOrdersQueryValidator : AbstractValidator<ExportOrdersQuery>
{
    public ExportOrdersQueryValidator()
    {
        RuleFor(x => x.Tab).IsInEnum();
        RuleFor(x => x.Status)
            .Must(s => s is null || Enum.IsDefined(typeof(OrderStatus), s.Value))
            .WithMessage("Invalid status.");
        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }
}
