using TaskManagement.Application.Common.Models;

namespace TaskManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    CurrentUser User { get; }
}