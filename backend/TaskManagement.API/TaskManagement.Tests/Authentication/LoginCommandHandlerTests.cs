//using FluentAssertions;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Moq;
//using TaskManagement.Application.Common.Interfaces;
//using TaskManagement.Application.Common.Models;
//using TaskManagement.Application.Features.Authentication.Login;
//using TaskManagement.Domain.Identity;
//using TaskManagement.Infrastructure.Authentication.Settings;

//namespace TaskManagement.Tests.Authentication.Login;

//public class LoginCommandHandlerTests
//{
//    private readonly Mock<UserManager<ApplicationUser>> _userManager;
//    private readonly Mock<IJwtService> _jwtService;
//    private readonly Mock<IRefreshTokenService> _refreshTokenService;
//    private readonly Mock<ISessionService> _sessionService;
//    private readonly Mock<IClientInfoService> _clientInfoService;

//    public LoginCommandHandlerTests()
//    {
//        _userManager = GetUserManagerMock();

//        _jwtService = new Mock<IJwtService>();
//        _refreshTokenService = new Mock<IRefreshTokenService>();
//        _sessionService = new Mock<ISessionService>();
//        _clientInfoService = new Mock<IClientInfoService>();
//    }

//    private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
//    {
//        return new Mock<UserManager<ApplicationUser>>(
//            Mock.Of<IUserStore<ApplicationUser>>(),
//            null,
//            null,
//            null,
//            null,
//            null,
//            null,
//            null,
//            null);
//    }

//    private LoginCommandHandler GetHandler()
//    {
//        return new LoginCommandHandler(
//            _userManager.Object,
//            _jwtService.Object,
//            _refreshTokenService.Object,
//            _sessionService.Object,
//            _clientInfoService.Object);
//    }

//    [Fact]
//    public async Task Should_Return_Error_When_User_Not_Found()
//    {
//        _userManager
//            .Setup(x => x.FindByEmailAsync("test@test.com"))
//            .ReturnsAsync((ApplicationUser?)null);

//        var handler = GetHandler();

//        var result = await handler.Handle(
//            new LoginCommand
//            {
//                Email = "test@test.com",
//                Password = "Password@123"
//            },
//            CancellationToken.None);

//        result.Succeeded.Should().BeFalse();
//        result.Message.Should().Be("Invalid email or password.");
//    }

//    [Fact]
//    public async Task Should_Return_Error_When_Email_Not_Verified()
//    {
//        var user = new ApplicationUser
//        {
//            Id = Guid.NewGuid().ToString(),
//            Email = "test@test.com",
//            UserName = "test",
//            IsActive = true
//        };

//        _userManager.Setup(x => x.FindByEmailAsync(user.Email))
//            .ReturnsAsync(user);

//        _userManager.Setup(x => x.IsEmailConfirmedAsync(user))
//            .ReturnsAsync(false);

//        var handler = GetHandler();

//        var result = await handler.Handle(
//            new LoginCommand
//            {
//                Email = user.Email!,
//                Password = "Password@123"
//            },
//            CancellationToken.None);

//        result.Succeeded.Should().BeFalse();
//        result.Message.Should().Be("Please verify your email before logging in.");
//    }

//    [Fact]
//    public async Task Should_Return_Error_When_Password_Is_Invalid()
//    {
//        var user = new ApplicationUser
//        {
//            Id = Guid.NewGuid().ToString(),
//            Email = "test@test.com",
//            UserName = "test",
//            IsActive = true
//        };

//        _userManager.Setup(x => x.FindByEmailAsync(user.Email))
//            .ReturnsAsync(user);

//        _userManager.Setup(x => x.IsEmailConfirmedAsync(user))
//            .ReturnsAsync(true);

//        _userManager.Setup(x => x.CheckPasswordAsync(user, "WrongPassword"))
//            .ReturnsAsync(false);

//        var handler = GetHandler();

//        var result = await handler.Handle(
//            new LoginCommand
//            {
//                Email = user.Email!,
//                Password = "WrongPassword"
//            },
//            CancellationToken.None);

//        result.Succeeded.Should().BeFalse();
//        result.Message.Should().Be("Invalid email or password.");
//    }
//}