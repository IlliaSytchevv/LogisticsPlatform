using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LogisticsPlatform.Infrastructure.Services;

public sealed class RefreshTokenStore(AppDbContext dbContext) : IRefreshTokenStore
{
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        dbContext.RefreshTokens.Add(token);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task<bool> TryRevokeAndReplaceAsync(
        Guid tokenId,
        string newTokenHash,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        int revoked = await dbContext.RefreshTokens
            .Where(x => x.Id == tokenId && !x.IsRevoked && x.ExpiryDateUtc > nowUtc)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.IsRevoked, true)
                    .SetProperty(x => x.RevokedAtUtc, nowUtc)
                    .SetProperty(x => x.ReplacedByTokenHash, newTokenHash),
                cancellationToken);

        return revoked > 0;
    }

    public async Task RevokeByHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        RefreshToken? stored = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (stored is null || stored.IsRevoked)
            return;

        stored.IsRevoked = true;
        stored.RevokedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
