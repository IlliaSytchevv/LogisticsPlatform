namespace LogisticsPlatform.Application.Services;

public static class PhotoStorageKeys
{
    public static string ForWarehouse(Guid orderId, Guid photoId, string contentType) =>
        $"warehouse/{orderId:N}/{photoId:N}{Extension(contentType)}";

    public static string ForOperation(Guid operationId, Guid photoId, string contentType) =>
        $"operation/{operationId:N}/{photoId:N}{Extension(contentType)}";

    private static string Extension(string contentType) =>
        contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            _ => ".bin"
        };
}
