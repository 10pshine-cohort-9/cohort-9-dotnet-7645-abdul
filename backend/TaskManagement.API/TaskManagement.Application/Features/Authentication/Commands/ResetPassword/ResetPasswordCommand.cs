using MediatR;
using TaskManagement.Application.Features.Authentication.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<ResetPasswordResponse>;