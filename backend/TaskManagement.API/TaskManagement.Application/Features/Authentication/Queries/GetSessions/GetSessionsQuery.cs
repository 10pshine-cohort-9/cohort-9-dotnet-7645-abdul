using MediatR;
namespace TaskManagement.Application.Features.Authentication.Queries.GetSessions;

public sealed record GetSessionsQuery
    : IRequest<List<SessionDto>>;