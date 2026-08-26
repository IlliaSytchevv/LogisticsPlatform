using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupply;

public sealed class AddOrderSupplyCommandValidator : AbstractValidator<AddOrderSupplyCommand>
{
    public AddOrderSupplyCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(64);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);
        
        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(64);
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
        
        RuleFor(x => x.UnitPriceCents)
            .GreaterThanOrEqualTo(0);
    }
}
