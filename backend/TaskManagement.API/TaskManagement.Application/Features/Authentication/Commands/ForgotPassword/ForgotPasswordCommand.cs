using MediatR;

namespace TaskManagement.Application.Features.Authentication.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Email
) : IRequest<ForgotPasswordResponse>;