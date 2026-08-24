using LogisticsPlatform.Domain.Entities;

namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IRefreshTokenStore
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);

    Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<bool> TryRevokeAndReplaceAsync(
        Guid tokenId,
        string newTokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task RevokeByHashAsync(string tokenHash, CancellationToken cancellationToken);
}
