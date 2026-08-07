using FluentAssertions;
using TaskManagement.Application.Features.Authentication.Commands.Register;
using Xunit;

namespace TaskManagement.Tests.Authentication;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }


    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var command = new RegisterCommand
        {
            FirstName = "Abdul",
            LastName = "Moiz",
            Username = "abdul",
            Email = "wrong-email",
            Password = "Password@123",
            ConfirmPassword = "Password@123"
        };


        var result = _validator.Validate(command);


        result.IsValid.Should().BeFalse();

        result.Errors
            .Should()
            .Contain(x => x.PropertyName == "Email");
    }



    [Fact]
    public void Should_Fail_When_Passwords_Do_Not_Match()
    {
        var command = new RegisterCommand
        {
            FirstName = "Abdul",
            LastName = "Moiz",
            Username = "abdul",
            Email = "test@test.com",
            Password = "Password@123",
            ConfirmPassword = "Password@456"
        };


        var result = _validator.Validate(command);


        result.IsValid.Should().BeFalse();
    }



    [Fact]
    public void Should_Fail_When_Password_Is_Weak()
    {
        var command = new RegisterCommand
        {
            FirstName = "Abdul",
            LastName = "Moiz",
            Username = "abdul",
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        };


        var result = _validator.Validate(command);


        result.IsValid.Should().BeFalse();
    }



    [Fact]
    public void Should_Pass_When_Register_Data_Is_Valid()
    {
        var command = new RegisterCommand
        {
            FirstName = "Abdul",
            LastName = "Moiz",
            Username = "abdul",
            Email = "test@test.com",
            Password = "Password@123",
            ConfirmPassword = "Password@123"
        };


        var result = _validator.Validate(command);


        result.IsValid.Should().BeTrue();
    }
}