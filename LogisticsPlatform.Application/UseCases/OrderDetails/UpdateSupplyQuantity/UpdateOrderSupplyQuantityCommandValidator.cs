using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateSupplyQuantity;

public sealed class UpdateOrderSupplyQuantityCommandValidator
    : AbstractValidator<UpdateOrderSupplyQuantityCommand>
{
    public UpdateOrderSupplyQuantityCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.SupplyId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
