using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Commands.ResetPassword;
using TaskManagement.Domain.Identity;
using Xunit;

namespace TaskManagement.Tests.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IRefreshTokenService> _refreshTokenService;
    private readonly Mock<ISessionService> _sessionService;
    private readonly Mock<ILogger<ResetPasswordCommandHandler>> _logger;

    public ResetPasswordCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        _userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        _refreshTokenService = new Mock<IRefreshTokenService>();
        _sessionService = new Mock<ISessionService>();
        _logger = new Mock<ILogger<ResetPasswordCommandHandler>>();
    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        _userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new ResetPasswordCommandHandler(
            _userManager.Object,
            _refreshTokenService.Object,
            _sessionService.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand(
                "abc@test.com",
                "123",
                "Password@123",
                "Password@123"),
            CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Should_Reset_Password_Successfully()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "abc@test.com",
            UserName = "abc",
            IsActive = true
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _userManager
            .Setup(x => x.ResetPasswordAsync(
                user,
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _refreshTokenService
            .Setup(x => x.RevokeAllAsync(
                user.Id,
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _sessionService
            .Setup(x => x.TerminateAllSessionsAsync(user.Id))
            .Returns(Task.CompletedTask);

        var handler = new ResetPasswordCommandHandler(
            _userManager.Object,
            _refreshTokenService.Object,
            _sessionService.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ResetPasswordCommand(
                user.Email!,
                "123",
                "Password@123",
                "Password@123"),
            CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);

        _refreshTokenService.Verify(
            x => x.RevokeAllAsync(
                user.Id,
                It.IsAny<string>()),
            Times.Once);

        _sessionService.Verify(
            x => x.TerminateAllSessionsAsync(user.Id),
            Times.Once);
    }
}