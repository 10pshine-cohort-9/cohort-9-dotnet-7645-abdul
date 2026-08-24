using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtTestController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public JwtTestController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpGet("generate")]
    public async Task<IActionResult> Generate()
    {
        var token = await _jwtService.GenerateAccessTokenAsync(
            Guid.NewGuid().ToString(),
            "testuser",
            "test@example.com",
            new List<string> { "Admin" });

        return Ok(new
        {
            AccessToken = token
        });
    }

    //[HttpPost("validate")]
    //public IActionResult Validate([FromBody] string token)
    //{
    //    var principal = _jwtService.ValidateToken(token);

    //    if (principal == null)
    //    {
    //        return BadRequest("Invalid Token");
    //    }

    //    return Ok(new
    //    {
    //        Claims = principal.Claims.Select(x => new
    //        {
    //            x.Type,
    //            x.Value
    //        })
    //    });
    //}
     
    [HttpPost("validate")]
    public IActionResult Validate([FromBody] string token)
    {
        // 👇 Breakpoint yahan lagao
        var receivedToken = token;

        var principal = _jwtService.ValidateToken(receivedToken);

        if (principal == null)
        {
            return BadRequest("Invalid Token");
        }

        return Ok(new
        {
            Claims = principal.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })
        });
    }
}