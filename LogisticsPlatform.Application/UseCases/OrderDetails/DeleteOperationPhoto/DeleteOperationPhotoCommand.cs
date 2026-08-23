using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.DeleteOperationPhoto;

public sealed record DeleteOperationPhotoCommand(
    Guid OrderId,
    Guid OperationId,
    Guid PhotoId) : ICommand<Result>;
