using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendTestEmail()
    {
        await _emailService.SendHtmlAsync(
            "abmoiz2776@gmail.com",
            "SMTP Test",
            """
            <h2>SMTP Working ✅</h2>
            <p>This email was sent successfully.</p>
            """);

        return Ok(new
        {
            Message = "Email sent successfully."
        });
    }
}