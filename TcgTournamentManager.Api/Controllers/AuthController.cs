using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _authService.LoginAsync(request, ct));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterOrganizerRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _authService.RegisterAsync(request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}