using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Infrastructure.Email.Models;

namespace TaskManagement.Infrastructure.Email.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> options,
        ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        await SendEmailAsync(
            to,
            subject,
            body,
            false,
            cancellationToken);
    }

    public async Task SendHtmlAsync(
        string to,
        string subject,
        string html,
        CancellationToken cancellationToken = default)
    {
        await SendEmailAsync(
            to,
            subject,
            html,
            true,
            cancellationToken);
    }

    private async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken)
    {
        try
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = isHtml ? body : null,
                TextBody = isHtml ? null : body
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                _settings.UseSSL
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.Auto,
                cancellationToken);

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password,
                cancellationToken);

            await smtp.SendAsync(
                email,
                cancellationToken);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);

            _logger.LogInformation(
                "Email sent successfully to {Recipient}",
                to);
        }
        catch (MailKit.CommandException ex)
        {
            _logger.LogError(
                ex,
                "SMTP command failed while sending email to {Recipient}",
                to);

            throw;
        }
        catch (MailKit.ProtocolException ex)
        {
            _logger.LogError(
                ex,
                "SMTP protocol error while sending email to {Recipient}",
                to);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while sending email to {Recipient}",
                to);

            throw;
        }
    }
}