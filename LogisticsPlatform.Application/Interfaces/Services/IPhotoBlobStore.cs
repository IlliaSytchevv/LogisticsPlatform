namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IPhotoBlobStore
{
    Task SaveAsync(string storageKey, byte[] content, CancellationToken cancellationToken);

    Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}
