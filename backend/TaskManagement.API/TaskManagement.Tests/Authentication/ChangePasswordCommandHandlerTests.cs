using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Commands.ChangePassword;
using TaskManagement.Domain.Identity;
using Xunit;

namespace TaskManagement.Tests.Application.Features.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IRefreshTokenService> _refreshTokenService;
    private readonly Mock<ISessionService> _sessionService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<ILogger<ChangePasswordCommandHandler>> _logger;

    public ChangePasswordCommandHandlerTests()
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
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _logger = new Mock<ILogger<ChangePasswordCommandHandler>>();

        var context = new DefaultHttpContext();

        context.User = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "user-id")
            ],
            "Test"));

        _httpContextAccessor
            .Setup(x => x.HttpContext)
            .Returns(context);
    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        _userManager
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new ChangePasswordCommandHandler(
            _userManager.Object,
            _refreshTokenService.Object,
            _sessionService.Object,
            _httpContextAccessor.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ChangePasswordCommand(
                "OldPassword@123",
                "NewPassword@123",
                "NewPassword@123"),
            CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Should_Change_Password_Successfully()
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
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.ChangePasswordAsync(
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

        var handler = new ChangePasswordCommandHandler(
            _userManager.Object,
            _refreshTokenService.Object,
            _sessionService.Object,
            _httpContextAccessor.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ChangePasswordCommand(
                "OldPassword@123",
                "NewPassword@123",
                "NewPassword@123"),
            CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);

        _userManager.Verify(
            x => x.ChangePasswordAsync(
                user,
                "OldPassword@123",
                "NewPassword@123"),
            Times.Once);

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