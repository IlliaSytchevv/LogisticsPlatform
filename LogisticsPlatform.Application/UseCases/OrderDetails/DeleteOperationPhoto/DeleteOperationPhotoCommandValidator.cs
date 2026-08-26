using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperationPhoto;

public sealed class DeleteOperationPhotoCommandValidator : AbstractValidator<DeleteOperationPhotoCommand>
{
    public DeleteOperationPhotoCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();
        
        RuleFor(x => x.OperationId)
            .NotEmpty();
        
        RuleFor(x => x.PhotoId)
            .NotEmpty();
    }
}
