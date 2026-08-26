using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteSupply;

public sealed class DeleteOrderSupplyCommandValidator : AbstractValidator<DeleteOrderSupplyCommand>
{
    public DeleteOrderSupplyCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.SupplyId)
            .NotEmpty();
    }
}
