using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;
using LogisticsPlatform.Domain.DTO.Orders.Detail;

namespace LogisticsPlatform.Application.UseCases.OrderDetails.AddWarehousePhoto;

public sealed record AddWarehousePhotoCommand(
    Guid OrderId,
    string Url,
    int? SortOrder) : ICommand<Result<OrderWarehousePhotoResponse>>;
