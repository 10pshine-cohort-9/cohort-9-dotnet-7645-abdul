using MediatR;

namespace TaskManagement.Application.Features.Authentication.Commands.LogoutAll;

public class LogoutAllCommand : IRequest<LogoutAllResponse>
{
    public string UserId { get; set; } = string.Empty;
}