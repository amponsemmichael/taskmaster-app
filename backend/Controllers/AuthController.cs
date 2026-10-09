using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskMaster.DTOs.Auth;
using TaskMaster.Extensions;
using TaskMaster.Services;

namespace TaskMaster.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        return Ok(await _authService.RegisterAsync(request));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        return Ok(await _authService.LoginAsync(request));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequestDto request)
    {
        return Ok(await _authService.RefreshAsync(request.RefreshToken));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequestDto request)
    {
        await _authService.RevokeAsync(request.RefreshToken);
        return NoContent();
    }
}
