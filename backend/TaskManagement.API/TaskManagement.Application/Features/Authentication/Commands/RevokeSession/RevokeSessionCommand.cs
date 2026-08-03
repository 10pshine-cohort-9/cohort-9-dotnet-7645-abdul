using MediatR;

namespace TaskManagement.Application.Features.Authentication.Commands.RevokeSession;

public sealed record RevokeSessionCommand(Guid SessionId)
    : IRequest<RevokeSessionResponse>;