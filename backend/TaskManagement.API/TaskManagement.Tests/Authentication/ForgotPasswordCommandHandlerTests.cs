using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Commands.ForgotPassword;
using TaskManagement.Domain.Identity;
using Xunit;

namespace TaskManagement.Tests.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IApplicationUrlService> _applicationUrlService;
    private readonly Mock<ILogger<ForgotPasswordCommandHandler>> _logger;

    public ForgotPasswordCommandHandlerTests()
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

        _emailService = new Mock<IEmailService>();
        _applicationUrlService = new Mock<IApplicationUrlService>();
        _logger = new Mock<ILogger<ForgotPasswordCommandHandler>>();

        _applicationUrlService
            .Setup(x => x.FrontendBaseUrl)
            .Returns("https://localhost:5173");
    }

    [Fact]
    public async Task Should_Return_Generic_Message_When_User_Not_Found()
    {
        // Arrange
        _userManager
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new ForgotPasswordCommandHandler(
            _userManager.Object,
            _emailService.Object,
            _applicationUrlService.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ForgotPasswordCommand("abc@test.com"),
            CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);

        _emailService.Verify(
            x => x.SendPasswordResetEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Send_Email_When_User_Exists()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "abc@test.com",
            UserName = "abc"
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _userManager
            .Setup(x => x.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("token");

        var handler = new ForgotPasswordCommandHandler(
            _userManager.Object,
            _emailService.Object,
            _applicationUrlService.Object,
            _logger.Object);

        // Act
        var result = await handler.Handle(
            new ForgotPasswordCommand(user.Email!),
            CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);

        _emailService.Verify(
            x => x.SendPasswordResetEmailAsync(
                user.Email!,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}