using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Phone).MaximumLength(64).When(x => x.Phone is not null);
        RuleFor(x => x.CustomerName).MaximumLength(256).When(x => x.CustomerName is not null);
        RuleFor(x => x.PrimaryReference).MaximumLength(64).When(x => x.PrimaryReference is not null);
        RuleFor(x => x.QuantityUnitLabel).MaximumLength(64).When(x => x.QuantityUnitLabel is not null);
        RuleFor(x => x.DockStatusLabel).MaximumLength(128).When(x => x.DockStatusLabel is not null);
        RuleFor(x => x.WarehouseNote).MaximumLength(2000).When(x => x.WarehouseNote is not null);
    }
}
