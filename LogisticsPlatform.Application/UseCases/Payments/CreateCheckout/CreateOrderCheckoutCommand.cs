using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Payments;
using Ardalis.Result;

namespace LogisticsPlatform.Application.UseCases.Payments.CreateCheckout;

public sealed record CreateOrderCheckoutCommand(Guid OrderId)
    : ICommand<Result<CreateCheckoutResponse>>;
