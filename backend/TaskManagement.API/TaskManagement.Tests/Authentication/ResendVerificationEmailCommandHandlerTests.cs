using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.ResendVerificationEmail;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Tests.Authentication;

public class ResendVerificationEmailCommandHandlerTests
{
    private readonly Mock<IEmailService> _emailMock = new();

    private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
    {
        return new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    [Fact]
    public async Task Should_Return_User_Not_Found()
    {
        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new ResendVerificationEmailCommandHandler(
            userManager.Object,
            _emailMock.Object);

        var result = await handler.Handle(
            new ResendVerificationEmailCommand
            {
                Email = "test@test.com"
            },
            CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("User not found.");
    }

    [Fact]
    public async Task Should_Return_Already_Verified()
    {
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            EmailConfirmed = true
        };

        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var handler = new ResendVerificationEmailCommandHandler(
            userManager.Object,
            _emailMock.Object);

        var result = await handler.Handle(
            new ResendVerificationEmailCommand
            {
                Email = user.Email!
            },
            CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Email is already verified.");
    }

    [Fact]
    public async Task Should_Send_Verification_Email()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            FirstName = "Abdul",
            Email = "test@test.com",
            EmailConfirmed = false
        };

        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
            .ReturnsAsync("token123");

        _emailMock
            .Setup(x => x.SendHtmlAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ResendVerificationEmailCommandHandler(
            userManager.Object,
            _emailMock.Object);

        // NOTE:
        // This test requires EmailVerification.html to be available
        // at runtime because the handler reads it from disk.

        var result = await handler.Handle(
            new ResendVerificationEmailCommand
            {
                Email = user.Email!
            },
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();

        _emailMock.Verify(x => x.SendHtmlAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}