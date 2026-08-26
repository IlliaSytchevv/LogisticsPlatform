using LogisticsPlatform.Application.DTO.Supplies;
using LogisticsPlatform.Application.Models.Supplies;

namespace LogisticsPlatform.Application.Extensions.Mapping.Supplies;

public static class SupplyCatalogMapper
{
    public static SupplyCatalogResponse ToResponse(SupplyCatalogData data) =>
        new(data.Items.Select(ToResponse).ToList());

    private static SupplyCatalogItemResponse ToResponse(SupplyCatalogItemData item) =>
        new(item.Id, item.Sku, item.Name, item.Category, item.PlatformPriceCents);
}
