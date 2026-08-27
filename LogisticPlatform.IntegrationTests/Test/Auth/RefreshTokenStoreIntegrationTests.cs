using System.Net.Http.Json;
using LogisticPlatform.IntegrationTests.Fixtures;
using LogisticPlatform.IntegrationTests.Helpers;
using LogisticsPlatform.Application.DTO.Authorization;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticPlatform.IntegrationTests.Test.Auth;

[Collection(IntegrationCollection.Name)]
public sealed class RefreshTokenStoreIntegrationTests(LogisticsApiFixture fixture)
{
    [Fact]
    public async Task AddAsync_ShouldPersistToken_WhenUserExists()
    {
        // Arrange
        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IRefreshTokenStore store = scope.ServiceProvider.GetRequiredService<IRefreshTokenStore>();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = SeedIds.TestUserId,
            TokenHash = $"hash-{Guid.NewGuid():N}",
            CreatedAtUtc = DateTime.UtcNow,
            ExpiryDateUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        // Act
        await store.AddAsync(token, CancellationToken.None);

        // Assert
        RefreshToken? stored = await db.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == token.Id);

        stored.ShouldNotBeNull();
        stored!.TokenHash.ShouldBe(token.TokenHash);
        stored.UserId.ShouldBe(SeedIds.TestUserId);
        stored.IsRevoked.ShouldBeFalse();
    }

    [Fact]
    public async Task FindByHashAsync_ShouldReturnToken_WhenHashMatches()
    {
        // Arrange
        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IRefreshTokenStore store = scope.ServiceProvider.GetRequiredService<IRefreshTokenStore>();

        string hash = $"find-{Guid.NewGuid():N}";
        await store.AddAsync(
            new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = SeedIds.TestUserId,
                TokenHash = hash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiryDateUtc = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            },
            CancellationToken.None);

        // Act
        RefreshToken? found = await store.FindByHashAsync(hash, CancellationToken.None);

        // Assert
        found.ShouldNotBeNull();
        found!.TokenHash.ShouldBe(hash);
        found.UserId.ShouldBe(SeedIds.TestUserId);
    }

    [Fact]
    public async Task TryRevokeAndReplaceAsync_ShouldRevokeAndSetReplacement_WhenTokenIsActive()
    {
        // Arrange
        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IRefreshTokenStore store = scope.ServiceProvider.GetRequiredService<IRefreshTokenStore>();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tokenId = Guid.NewGuid();
        string oldHash = $"old-{Guid.NewGuid():N}";
        string newHash = $"new-{Guid.NewGuid():N}";
        DateTime now = DateTime.UtcNow;

        await store.AddAsync(
            new RefreshToken
            {
                Id = tokenId,
                UserId = SeedIds.TestUserId,
                TokenHash = oldHash,
                CreatedAtUtc = now.AddMinutes(-5),
                ExpiryDateUtc = now.AddDays(7),
                IsRevoked = false
            },
            CancellationToken.None);

        // Act
        bool revoked = await store.TryRevokeAndReplaceAsync(tokenId, newHash, now, CancellationToken.None);

        // Assert
        revoked.ShouldBeTrue();

        RefreshToken stored = await db.RefreshTokens
            .AsNoTracking()
            .SingleAsync(x => x.Id == tokenId);

        stored.IsRevoked.ShouldBeTrue();
        stored.RevokedAtUtc.ShouldNotBeNull();
        stored.ReplacedByTokenHash.ShouldBe(newHash);
    }

    [Fact]
    public async Task TryRevokeAndReplaceAsync_ShouldReturnFalse_WhenTokenAlreadyRevoked()
    {
        // Arrange
        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        IRefreshTokenStore store = scope.ServiceProvider.GetRequiredService<IRefreshTokenStore>();

        var tokenId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        await store.AddAsync(
            new RefreshToken
            {
                Id = tokenId,
                UserId = SeedIds.TestUserId,
                TokenHash = $"revoked-{Guid.NewGuid():N}",
                CreatedAtUtc = now.AddMinutes(-5),
                ExpiryDateUtc = now.AddDays(7),
                IsRevoked = true,
                RevokedAtUtc = now.AddMinutes(-1)
            },
            CancellationToken.None);

        // Act
        bool revoked = await store.TryRevokeAndReplaceAsync(
            tokenId,
            $"replacement-{Guid.NewGuid():N}",
            now,
            CancellationToken.None);

        // Assert
        revoked.ShouldBeFalse();
    }

    [Fact]
    public async Task LoginAndRefresh_ShouldRotateStoredRefreshToken_WhenCookieIsValid()
    {
        // Arrange — dedicated client so cookies do not leak across tests
        using HttpClient client = fixture.Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });

        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest("AdminUser", "Test123!"));
        loginResponse.EnsureSuccessStatusCode();

        await using AsyncServiceScope scope = fixture.Factory.Services.CreateAsyncScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        RefreshToken latestBefore = await db.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == SeedIds.TestUserId && !x.IsRevoked)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstAsync();

        // Act
        HttpResponseMessage refreshResponse = await client.PostAsync("/api/v1/auth/refresh-token", content: null);

        // Assert
        refreshResponse.EnsureSuccessStatusCode();

        RefreshToken rotated = await db.RefreshTokens
            .AsNoTracking()
            .SingleAsync(x => x.Id == latestBefore.Id);

        rotated.IsRevoked.ShouldBeTrue();
        rotated.ReplacedByTokenHash.ShouldNotBeNullOrWhiteSpace();

        bool hasReplacement = await db.RefreshTokens.AnyAsync(x =>
            x.UserId == SeedIds.TestUserId &&
            !x.IsRevoked &&
            x.TokenHash == rotated.ReplacedByTokenHash);

        hasReplacement.ShouldBeTrue();
    }
}
