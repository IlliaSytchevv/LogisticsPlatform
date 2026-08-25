using Ardalis.Result;
using LogisticsPlatform.Application.Interfaces.Services;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Application.UseCases.Auth.Login;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LogisticPlatform.UnitTests.Auth;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IUserManagerWrapper> _users = new();
    private readonly Mock<IJwtTokenService> _jwt = new();
    private readonly Mock<IRefreshTokenStore> _refreshStore = new();
    private readonly Mock<ILogger<LoginCommandHandler>> _logger = new();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "test",
            Audience = "test",
            SecretKey = "unit-test-secret-key-at-least-32-chars!",
            RefreshExpirationInDays = 7
        });

        _sut = new LoginCommandHandler(
            _users.Object,
            _jwt.Object,
            _refreshStore.Object,
            options,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsMissing()
    {
        _users.Setup(x => x.FindByNameAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

        Result<LogisticsPlatform.Application.Models.Auth.LoginTokensData> result =
            await _sut.Handle(new LoginCommand("ghost", "pwd"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _jwt.Verify(x => x.GenerateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()), Times.Never);
        _refreshStore.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsInactive()
    {
        var user = CreateUser(isActive: false);
        _users.Setup(x => x.FindByNameAsync(user.UserName!)).ReturnsAsync(user);

        var result = await _sut.Handle(new LoginCommand(user.UserName!, "Test123!"), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _users.Verify(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        var user = CreateUser(isActive: true);
        _users.Setup(x => x.FindByNameAsync(user.UserName!)).ReturnsAsync(user);
        _users.Setup(x => x.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(new LoginCommand(user.UserName!, "wrong"), CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _refreshStore.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnTokens_WhenCredentialsAreValid()
    {
        var user = CreateUser(isActive: true);
        _users.Setup(x => x.FindByNameAsync(user.UserName!)).ReturnsAsync(user);
        _users.Setup(x => x.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
        _users.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        _jwt.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>())).Returns("access.jwt");
        _jwt.Setup(x => x.GenerateRefreshToken(user.Id)).Returns("refresh.jwt");
        _jwt.Setup(x => x.HashRefreshToken("refresh.jwt")).Returns("hash");

        var result = await _sut.Handle(new LoginCommand(user.UserName!, "Test123!"), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldBe("access.jwt");
        result.Value.RefreshToken.ShouldBe("refresh.jwt");
        _refreshStore.Verify(
            x => x.AddAsync(
                It.Is<RefreshToken>(t =>
                    t.UserId == user.Id &&
                    t.TokenHash == "hash" &&
                    !t.IsRevoked),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static ApplicationUser CreateUser(bool isActive) =>
        new()
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "testuser",
            DisplayName = "User 1",
            Initials = "U1",
            ExternalId = "ext",
            IsActive = isActive
        };
}