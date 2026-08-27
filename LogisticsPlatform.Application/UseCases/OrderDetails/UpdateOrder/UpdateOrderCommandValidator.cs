using FluentValidation;
using LogisticsPlatform.Domain.Enums;
using LogisticsPlatform.Domain.Orders;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.UpdateOrder;

public sealed class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Number)
            .MaximumLength(32)
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .When(x => x.Number is not null)
            .WithMessage("Order number cannot be empty.");

        RuleFor(x => x.Number)
            .Must(n => !OrderNumberRules.IsDraftNumber(n))
            .When(x =>
                x.Number is not null &&
                x.Status is not null &&
                x.Status != OrderStatus.Draft)
            .WithMessage("Rename the order number before leaving Draft (cannot keep a DRAFT-… number).");

        RuleFor(x => x.CustomerName)
            .MaximumLength(256)
            .When(x => x.CustomerName is not null);

        RuleFor(x => x.PrimaryReference)
            .MaximumLength(64)
            .When(x => x.PrimaryReference is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(64)
            .When(x => x.Phone is not null);

        RuleFor(x => x.TrailerType)
            .MaximumLength(64)
            .When(x => x.TrailerType is not null);

        RuleFor(x => x.TruckNumber)
            .MaximumLength(64)
            .When(x => x.TruckNumber is not null);

        RuleFor(x => x.TrailerNumber)
            .MaximumLength(64)
            .When(x => x.TrailerNumber is not null);

        RuleFor(x => x.DockCode)
            .MaximumLength(32)
            .When(x => x.DockCode is not null);

        RuleFor(x => x.DockBay)
            .MaximumLength(32)
            .When(x => x.DockBay is not null);

        RuleFor(x => x.WarehouseNote)
            .MaximumLength(2000)
            .When(x => x.WarehouseNote is not null);

        RuleFor(x => x.StockStatusLabel)
            .MaximumLength(64)
            .When(x => x.StockStatusLabel is not null);

        RuleFor(x => x.LoadingStatusLabel)
            .MaximumLength(64)
            .When(x => x.LoadingStatusLabel is not null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
