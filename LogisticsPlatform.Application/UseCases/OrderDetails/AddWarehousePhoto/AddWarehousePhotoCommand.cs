using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed record AddWarehousePhotoCommand(
    Guid OrderId,
    string FileName,
    string ContentType,
    byte[] Content) : ICommand<Result<OrderWarehousePhotoResponse>>;
