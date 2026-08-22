using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteWarehousePhoto;

public sealed record DeleteWarehousePhotoCommand(
    Guid OrderId,
    Guid PhotoId) : ICommand<Result>;
