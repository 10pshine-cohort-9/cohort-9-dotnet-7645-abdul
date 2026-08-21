namespace TaskManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    Task SendHtmlAsync(
        string to,
        string subject,
        string html,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(
        string email,
        string resetLink,
        CancellationToken cancellationToken = default);
}