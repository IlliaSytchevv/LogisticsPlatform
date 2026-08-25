using System.Security.Claims;
using Ardalis.Result;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Application.Models.Auth;
using LogisticsPlatform.Application.UseCases.Auth.RefreshToken;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace LogisticPlatform.UnitTests.Auth;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IJwtTokenService> _jwt = new();
    private readonly Mock<IRefreshTokenStore> _refreshStore = new();
    private readonly Mock<IUserManagerWrapper> _users = new();
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTests()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "test",
            Audience = "test",
            SecretKey = "unit-test-secret-key-at-least-32-chars!",
            RefreshExpirationInDays = 7
        });

        _sut = new RefreshTokenCommandHandler(
            _jwt.Object,
            _refreshStore.Object,
            _users.Object,
            options);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_ShouldReturnUnauthorized_WhenCookieTokenIsMissing(string? cookie)
    {
        var command = new RefreshTokenCommand(cookie!);

        Result<RefreshTokensData> result = await _sut.Handle(command, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRefreshJwtIsInvalid()
    {
        ClaimsPrincipal? principal = null;
        _jwt.Setup(x => x.TryValidateRefreshToken("bad", out principal)).Returns(false);

        var result = await _sut.Handle(new RefreshTokenCommand("bad"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenStoredTokenIsMissing()
    {
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenStoredTokenIsRevoked()
    {
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateStoredToken(isRevoked: true, expiryUtc: DateTime.UtcNow.AddDays(1)));

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenStoredTokenIsExpired()
    {
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateStoredToken(isRevoked: false, expiryUtc: DateTime.UtcNow.AddMinutes(-1)));

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsMissingOrInactive()
    {
        var stored = CreateStoredToken(isRevoked: false, expiryUtc: DateTime.UtcNow.AddDays(1));
        
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);
        
        _users.Setup(x => x.FindByIdAsync(stored.UserId.ToString())).ReturnsAsync((ApplicationUser?)null);

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRevokeAndReplaceFails()
    {
        var user = CreateActiveUser();
        var stored = CreateStoredToken(isRevoked: false, expiryUtc: DateTime.UtcNow.AddDays(1), userId: user.Id);
        
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);
        
        _users.Setup(x => x.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _users.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        _jwt.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>())).Returns("new-access");
        _jwt.Setup(x => x.GenerateRefreshToken(user.Id)).Returns("new-refresh");
        _jwt.Setup(x => x.HashRefreshToken("new-refresh")).Returns("new-hash");
        
        _refreshStore
            .Setup(x => x.TryRevokeAndReplaceAsync(
                stored.Id,
                "new-hash",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _refreshStore.Verify(
            x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRotateTokens_WhenRefreshIsValid()
    {
        var user = CreateActiveUser();
        var stored = CreateStoredToken(isRevoked: false, expiryUtc: DateTime.UtcNow.AddDays(1), userId: user.Id);
        
        SetupValidJwt("cookie-token");
        _jwt.Setup(x => x.HashRefreshToken("cookie-token")).Returns("old-hash");
        _refreshStore
            .Setup(x => x.FindByHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stored);
        
        _users.Setup(x => x.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _users.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        _jwt.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>())).Returns("new-access");
        _jwt.Setup(x => x.GenerateRefreshToken(user.Id)).Returns("new-refresh");
        _jwt.Setup(x => x.HashRefreshToken("new-refresh")).Returns("new-hash");
        
        _refreshStore
            .Setup(x => x.TryRevokeAndReplaceAsync(
                stored.Id,
                "new-hash",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.Handle(new RefreshTokenCommand("cookie-token"), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldBe("new-access");
        result.Value.RefreshToken.ShouldBe("new-refresh");
        _refreshStore.Verify(
            x => x.AddAsync(
                It.Is<RefreshToken>(t => t.TokenHash == "new-hash" && t.UserId == user.Id && !t.IsRevoked),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void SetupValidJwt(string token)
    {
        ClaimsPrincipal? principal = new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ]));
        _jwt.Setup(x => x.TryValidateRefreshToken(token, out principal)).Returns(true);
    }

    private static RefreshToken CreateStoredToken(bool isRevoked, DateTime expiryUtc, Guid? userId = null) =>
        new()
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            UserId = userId ?? Guid.Parse("11111111-1111-1111-1111-111111111111"),
            TokenHash = "old-hash",
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            ExpiryDateUtc = expiryUtc,
            IsRevoked = isRevoked
        };

    private static ApplicationUser CreateActiveUser() =>
        new()
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "testuser",
            DisplayName = "User 1",
            Initials = "U1",
            ExternalId = "ext",
            IsActive = true
        };
}