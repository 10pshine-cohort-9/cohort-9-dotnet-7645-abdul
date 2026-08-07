using MediatR;

namespace TaskManagement.Application.Features.Authentication.Commands.Logout;

public class LogoutCommand : IRequest<LogoutResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}