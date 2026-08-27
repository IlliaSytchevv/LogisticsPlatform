using Ardalis.Result;
using LogisticsPlatform.Application.DTO.Authorization;
using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Application.UseCases.Auth.Register;
using LogisticsPlatform.Domain.Entities;
using LogisticsPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogisticPlatform.UnitTests.Auth;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserManagerWrapper> _users = new();
    private readonly Mock<ILogger<RegisterUserCommandHandler>> _logger = new();
    private readonly RegisterUserCommandHandler _sut;

    public RegisterUserCommandHandlerTests()
    {
        _sut = new RegisterUserCommandHandler(_users.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalid_WhenCreateFails()
    {
        _users
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Username taken"
            }));

        Result<RegisterResponse> result = await _sut.Handle(
            new RegisterUserCommand("Jane Doe", "jane@test.local", "Test123!", "Dispatcher"),
            CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage == "Username taken");
        _users.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMapAdminRoleAndBuildInitials_WhenNameHasTwoParts()
    {
        ApplicationUser? created = null;
        
        _users
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .Callback<ApplicationUser, string>((user, _) => created = user)
            .ReturnsAsync(IdentityResult.Success);
        _users
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(
            new RegisterUserCommand("Jane Doe", "jane@test.local", "Test123!", "Admin"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        created.ShouldNotBeNull();
        created!.Initials.ShouldBe("JD");
        created.Role.ShouldBe(UserRole.Admin);
        created.DisplayName.ShouldBe("Jane Doe");
        _users.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapDriverRole_WhenRoleIsDriver()
    {
        ApplicationUser? created = null;
        
        _users
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .Callback<ApplicationUser, string>((user, _) => created = user)
            .ReturnsAsync(IdentityResult.Success);
        _users
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Driver"))
            .Returns(Task.CompletedTask);

        var result = await _sut.Handle(
            new RegisterUserCommand("Solo", "solo@test.local", "Test123!", "Driver"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        created.ShouldNotBeNull();
        created!.Role.ShouldBe(UserRole.Driver);
        created.Initials.ShouldBe("SO");
    }

    [Fact]
    public async Task Handle_ShouldBuildSingleLetterInitial_WhenNameIsOneCharacter()
    {
        ApplicationUser? created = null;
        
        _users
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!"))
            .Callback<ApplicationUser, string>((user, _) => created = user)
            .ReturnsAsync(IdentityResult.Success);
        _users
            .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Dispatcher"))
            .Returns(Task.CompletedTask);

        await _sut.Handle(
            new RegisterUserCommand("A", "a@test.local", "Test123!", "Dispatcher"),
            CancellationToken.None);

        created.ShouldNotBeNull();
        created!.Initials.ShouldBe("A");
        created.Role.ShouldBe(UserRole.Dispatcher);
    }
}
