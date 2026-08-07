using MediatR;
using TaskManagement.Application.Features.Authentication.Commands.Login;

namespace TaskManagement.Application.Features.Authentication.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}