using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Services;

namespace TaskManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(model);
        if (!result.IsAuthenticated) return BadRequest(result.Message);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(model);
        if (!result.IsAuthenticated) return Unauthorized(result.Message);

        return Ok(result);
    }
}