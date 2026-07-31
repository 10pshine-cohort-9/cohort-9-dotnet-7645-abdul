using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Authentication.Commands.Register;
using TaskManagement.Application.Features.Authentication.VerifyEmail;
using TaskManagement.Application.Features.Authentication.ResendVerificationEmail;
namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(
    [FromQuery] string userId,
    [FromQuery] string token)
    {
        var result = await _mediator.Send(
            new VerifyEmailCommand
            {
                UserId = userId,
                Token = token
            });

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(
    ResendVerificationEmailCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
}