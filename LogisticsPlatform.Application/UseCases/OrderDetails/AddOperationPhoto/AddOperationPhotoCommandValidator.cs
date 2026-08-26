using FluentValidation;
using LogisticsPlatform.Application.Extensions;
using LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddOperationPhoto;

public sealed class AddOperationPhotoCommandValidator : AbstractValidator<AddOperationPhotoCommand>
{
    public AddOperationPhotoCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.OperationId)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(c => c.Length <= AddWarehousePhotoCommandValidator.MaxFileBytes)
            .WithMessage($"File size must be <= {AddWarehousePhotoCommandValidator.MaxFileBytes / (1024 * 1024)} MB.")
            .Must(c => ImageContentTypeDetector.TryDetect(c, out _))
            .WithMessage("Only jpeg, png, webp and gif images are allowed.");
    }
}
