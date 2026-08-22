using FluentValidation;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed class AddWarehousePhotoCommandValidator : AbstractValidator<AddWarehousePhotoCommand>
{
    public AddWarehousePhotoCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(1024);
    }
}
