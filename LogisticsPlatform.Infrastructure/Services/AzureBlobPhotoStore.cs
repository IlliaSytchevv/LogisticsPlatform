using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Options;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class AzureBlobPhotoStore : IPhotoBlobStore
{
    private readonly BlobContainerClient _container;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public AzureBlobPhotoStore(IOptions<PhotoStorageOptions> options)
    {
        PhotoStorageOptions settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException("PhotoStorage:ConnectionString is missing.");

        if (string.IsNullOrWhiteSpace(settings.ContainerName))
            throw new InvalidOperationException("PhotoStorage:ContainerName is missing.");

        var service = new BlobServiceClient(settings.ConnectionString);
        _container = service.GetBlobContainerClient(settings.ContainerName);
    }

    public async Task SaveAsync(string storageKey, byte[] content, CancellationToken cancellationToken)
    {
        string key = NormalizeKey(storageKey);
        await EnsureContainerAsync(cancellationToken);

        BlobClient blob = _container.GetBlobClient(key);
        await using var stream = new MemoryStream(content, writable: false);
        await blob.UploadAsync(stream, overwrite: true, cancellationToken);
    }

    public async Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
    {
        string key = NormalizeKey(storageKey);
        await EnsureContainerAsync(cancellationToken);

        BlobClient blob = _container.GetBlobClient(key);
        try
        {
            Response<BlobDownloadStreamingResult> response = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        string key = NormalizeKey(storageKey);
        await EnsureContainerAsync(cancellationToken);

        BlobClient blob = _container.GetBlobClient(key);
        await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    private async Task EnsureContainerAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        };

        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            await _container.CreateIfNotExistsAsync(
                PublicAccessType.None,
                cancellationToken: cancellationToken);
            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private static string NormalizeKey(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Contains("..", StringComparison.Ordinal)
            || Path.IsPathRooted(storageKey)
            || storageKey.StartsWith('/')
            || storageKey.StartsWith('\\'))
        {
            throw new ArgumentException("Invalid storage key.", nameof(storageKey));
        }

        return storageKey.Replace('\\', '/');
    }
}