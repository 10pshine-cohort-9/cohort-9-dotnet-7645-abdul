using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Authentication.Login;
using TaskManagement.Domain.Identity;
using Xunit;

namespace TaskManagement.Tests.Authentication.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IRefreshTokenService> _refreshTokenService;
    private readonly Mock<ISessionService> _sessionService;
    private readonly Mock<IClientInfoService> _clientInfoService;
    private readonly Mock<ILogger<LoginCommandHandler>> _logger;

    public LoginCommandHandlerTests()
    {
        _userManager = GetUserManagerMock();
        _signInManager = GetSignInManagerMock();

        _jwtService = new Mock<IJwtService>();
        _refreshTokenService = new Mock<IRefreshTokenService>();
        _sessionService = new Mock<ISessionService>();
        _clientInfoService = new Mock<IClientInfoService>();
        _logger = new Mock<ILogger<LoginCommandHandler>>();
    }

    private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
    {
        return new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    private Mock<SignInManager<ApplicationUser>> GetSignInManagerMock()
    {
        return new Mock<SignInManager<ApplicationUser>>(
            _userManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null,
            null,
            null,
            null);
    }

    private LoginCommandHandler GetHandler()
    {
        return new LoginCommandHandler(
            _userManager.Object,
            _signInManager.Object,
            _jwtService.Object,
            _refreshTokenService.Object,
            _sessionService.Object,
            _clientInfoService.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        _userManager
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = "test@test.com",
                Password = "Password@123"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Should_Return_Error_When_User_Is_Inactive()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test",
            IsActive = false
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = user.Email!,
                Password = "Password@123"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Your account is inactive.");
    }

    [Fact]
    public async Task Should_Return_Error_When_User_Is_Locked()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test",
            IsActive = true
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(true);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = user.Email!,
                Password = "Password@123"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Your account is locked. Please try again later.");
    }

    [Fact]
    public async Task Should_Return_Error_When_Email_Not_Verified()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test",
            IsActive = true
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(false);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(false);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = user.Email!,
                Password = "Password@123"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Please verify your email before logging in.");
    }

    [Fact]
    public async Task Should_Return_Error_When_Password_Is_Invalid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test",
            IsActive = true
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(false);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _signInManager
            .Setup(x => x.CheckPasswordSignInAsync(
                user,
                "WrongPassword",
                true))
            .ReturnsAsync(SignInResult.Failed);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = user.Email!,
                Password = "WrongPassword"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Should_Return_Error_When_User_Gets_Locked_During_Login()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test",
            IsActive = true
        };

        _userManager
            .Setup(x => x.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManager
            .Setup(x => x.IsLockedOutAsync(user))
            .ReturnsAsync(false);

        _userManager
            .Setup(x => x.IsEmailConfirmedAsync(user))
            .ReturnsAsync(true);

        _signInManager
            .Setup(x => x.CheckPasswordSignInAsync(
                user,
                "WrongPassword",
                true))
            .ReturnsAsync(SignInResult.LockedOut);

        var handler = GetHandler();

        // Act
        var result = await handler.Handle(
            new LoginCommand
            {
                Email = user.Email!,
                Password = "WrongPassword"
            },
            CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Message.Should().Be("Your account has been locked. Please try again later.");
    }
}