using MediatR;
using TaskManagement.Application.Features.Authentication.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<ChangePasswordResponse>;