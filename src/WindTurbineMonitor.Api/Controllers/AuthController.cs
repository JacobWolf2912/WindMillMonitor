using Microsoft.AspNetCore.Mvc;
using WindTurbineMonitor.Api.Services;

namespace WindTurbineMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required");

        logger.LogInformation("Login attempt for user {Username}", request.Username);

        var token = authService.GenerateToken(request.Username);

        return Ok(new LoginResponse
        {
            Token = token,
            Username = request.Username,
            ExpiresIn = 3600 // 1 hour in seconds
        });
    }
}

public record LoginRequest(string Username);

public record LoginResponse
{
    public required string Token { get; init; }
    public required string Username { get; init; }
    public required int ExpiresIn { get; init; }
}
