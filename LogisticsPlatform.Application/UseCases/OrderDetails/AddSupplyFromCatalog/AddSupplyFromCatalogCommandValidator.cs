using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddSupplyFromCatalog;

public sealed class AddSupplyFromCatalogCommandValidator
    : AbstractValidator<AddSupplyFromCatalogCommand>
{
    public AddSupplyFromCatalogCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.CatalogItemId)
            .NotEmpty();
        
        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 10_000);
    }
}
