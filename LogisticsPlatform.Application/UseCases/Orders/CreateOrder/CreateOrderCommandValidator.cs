using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum();
        
        RuleFor(x => x.HubId)
            .NotEmpty();
        
        RuleFor(x => x.CreatedByUserId)
            .NotEmpty();
        
        RuleFor(x => x.DestinationCity)
            .MaximumLength(128)
            .When(x => x.DestinationCity is not null);
        
        RuleFor(x => x.DestinationRegion)
            .MaximumLength(64)
            .When(x => x.DestinationRegion is not null);
        
        RuleFor(x => x.PrimaryReference)
            .MaximumLength(64)
            .When(x => x.PrimaryReference is not null);
        
        RuleForEach(x => x.Supplies)
            .ChildRules(line =>
            {
                line.RuleFor(l => l.CatalogItemId)
                    .NotEmpty();
                
                line.RuleFor(l => l.Quantity)
                    .InclusiveBetween(1, 10_000);
            })
            .When(x => x.Supplies is not null);
    }
}
