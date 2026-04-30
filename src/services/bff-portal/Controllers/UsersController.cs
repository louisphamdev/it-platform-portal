using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.User.Models;
using Modules.User.Services;

namespace Bff.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}/profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile(Guid userId)
    {
        var profile = await _userService.GetProfileAsync(userId);
        if (profile == null)
            return NotFound(new { message = "Profile not found" });
        return Ok(profile);
    }

    [HttpPut("{userId:guid}/profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(Guid userId, [FromBody] UpdateProfileRequest request)
    {
        var profile = await _userService.UpdateProfileAsync(userId, request);
        if (profile == null)
            return NotFound(new { message = "Profile not found" });
        return Ok(profile);
    }

    [HttpPost("{userId:guid}/change-password")]
    public async Task<ActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(userId, request);
        if (!result)
            return BadRequest(new { message = "Failed to change password" });
        return Ok(new { message = "Password changed successfully" });
    }

    [HttpPost("{userId:guid}/sessions")]
    public async Task<ActionResult> CreateSession(Guid userId, [FromBody] CreateSessionRequest request)
    {
        var session = await _userService.CreateSessionAsync(
            userId,
            request.RefreshToken,
            request.DeviceInfo,
            request.IpAddress);
        if (session == null)
            return BadRequest(new { message = "Failed to create session" });
        return Ok(session);
    }

    [HttpDelete("sessions/{refreshToken}")]
    public async Task<ActionResult> RevokeSession(string refreshToken)
    {
        var result = await _userService.RevokeSessionAsync(refreshToken);
        if (!result)
            return BadRequest(new { message = "Failed to revoke session" });
        return NoContent();
    }

    [HttpDelete("{userId:guid}/sessions")]
    public async Task<ActionResult> RevokeAllSessions(Guid userId)
    {
        var result = await _userService.RevokeAllSessionsAsync(userId);
        if (!result)
            return BadRequest(new { message = "Failed to revoke sessions" });
        return NoContent();
    }
}

public class CreateSessionRequest
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
}
