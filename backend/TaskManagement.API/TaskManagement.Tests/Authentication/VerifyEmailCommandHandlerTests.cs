using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using TaskManagement.Application.Features.Authentication.VerifyEmail;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Tests.Authentication;

public class VerifyEmailCommandHandlerTests
{
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
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = new VerifyEmailCommandHandler(userManager.Object);

        var result = await handler.Handle(
            new VerifyEmailCommand
            {
                UserId = "1",
                Token = "abc"
            },
            CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("User not found.");
        result.IsEmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Return_Already_Verified()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            EmailConfirmed = true
        };

        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        var handler = new VerifyEmailCommandHandler(userManager.Object);

        var result = await handler.Handle(
            new VerifyEmailCommand
            {
                UserId = "1",
                Token = "abc"
            },
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.IsEmailVerified.Should().BeTrue();
        result.Message.Should().Be("Email is already verified.");
    }

    [Fact]
    public async Task Should_Return_Invalid_Token()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            EmailConfirmed = false
        };

        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError
                {
                    Description = "Invalid token."
                }));

        var handler = new VerifyEmailCommandHandler(userManager.Object);

        var result = await handler.Handle(
            new VerifyEmailCommand
            {
                UserId = "1",
                Token = "abc"
            },
            CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.IsEmailVerified.Should().BeFalse();
        result.Message.Should().Be("Invalid token.");
    }

    [Fact]
    public async Task Should_Verify_Email()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            EmailConfirmed = false
        };

        var userManager = GetUserManagerMock();

        userManager
            .Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new VerifyEmailCommandHandler(userManager.Object);

        var result = await handler.Handle(
            new VerifyEmailCommand
            {
                UserId = "1",
                Token = "abc"
            },
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.IsEmailVerified.Should().BeTrue();
        result.Message.Should().Be("Email verified successfully.");
    }
}