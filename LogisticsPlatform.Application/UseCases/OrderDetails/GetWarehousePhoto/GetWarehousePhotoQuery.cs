using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Application.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.GetWarehousePhoto;

public sealed record GetWarehousePhotoQuery(Guid OrderId, Guid PhotoId)
    : IQuery<Result<OrderFileResponse>>;
