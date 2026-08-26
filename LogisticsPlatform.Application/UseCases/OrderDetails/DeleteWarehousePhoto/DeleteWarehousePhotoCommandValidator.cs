using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;

public sealed class DeleteWarehousePhotoCommandValidator : AbstractValidator<DeleteWarehousePhotoCommand>
{
    public DeleteWarehousePhotoCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.PhotoId)
            .NotEmpty();
    }
}
