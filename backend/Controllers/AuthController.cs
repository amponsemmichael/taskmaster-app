using Microsoft.AspNetCore.Mvc;
using TaskMaster.DTOs.Auth;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Controllers;

[ApiController]
[Route("api/auth")]
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
}
