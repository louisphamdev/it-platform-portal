using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Models;
using Modules.Auth.Services;

namespace Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid username or password" });
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (result == null)
            return Unauthorized(new { message = "Invalid refresh token" });
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result == null)
            return Conflict(new { message = "Username or email already exists" });
        return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("users/{username}")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username)
    {
        var user = await _authService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost("users/{id:guid}/lock")]
    public async Task<ActionResult> LockUser(Guid id)
    {
        await _authService.LockUserAsync(id);
        return NoContent();
    }

    [HttpPost("users/{id:guid}/unlock")]
    public async Task<ActionResult> UnlockUser(Guid id)
    {
        await _authService.UnlockUserAsync(id);
        return NoContent();
    }

    [HttpGet("users/{id:guid}/roles")]
    public async Task<ActionResult<List<string>>> GetUserRoles(Guid id)
    {
        var roles = await _authService.GetUserRolesAsync(id);
        return Ok(roles);
    }
}
