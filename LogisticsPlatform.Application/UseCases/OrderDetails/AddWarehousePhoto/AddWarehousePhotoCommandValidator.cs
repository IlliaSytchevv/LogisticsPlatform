using FluentValidation;
using LogisticsPlatform.Application.Extensions;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed class AddWarehousePhotoCommandValidator : AbstractValidator<AddWarehousePhotoCommand>
{
    public const long MaxFileBytes = 5 * 1024 * 1024;

    public AddWarehousePhotoCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(c => c.Length <= MaxFileBytes)
            .WithMessage($"File size must be <= {MaxFileBytes / (1024 * 1024)} MB.")
            .Must(c => ImageContentTypeDetector.TryDetect(c, out _))
            .WithMessage("Only jpeg, png, webp and gif images are allowed.");
    }
}
