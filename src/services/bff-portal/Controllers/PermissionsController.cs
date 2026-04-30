using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Permission.Models;
using Modules.Permission.Services;

namespace Bff.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetAllPermissions()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("role/{roleId:guid}")]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissionsByRole(Guid roleId)
    {
        var permissions = await _permissionService.GetPermissionsByRoleAsync(roleId);
        return Ok(permissions);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<string>>> GetPermissionCodesByUser(Guid userId)
    {
        var permissions = await _permissionService.GetPermissionCodesByUserAsync(userId);
        return Ok(permissions);
    }

    [HttpPost("assign")]
    public async Task<ActionResult> AssignPermissions([FromBody] AssignPermissionsRequest request)
    {
        var result = await _permissionService.AssignPermissionsAsync(request);
        if (!result)
            return BadRequest(new { message = "Failed to assign permissions" });
        return Ok(new { message = "Permissions assigned successfully" });
    }

    [HttpDelete("role/{roleId:guid}/permission/{permissionId:guid}")]
    public async Task<ActionResult> RemovePermission(Guid roleId, Guid permissionId)
    {
        var result = await _permissionService.RemovePermissionAsync(roleId, permissionId);
        if (!result)
            return BadRequest(new { message = "Failed to remove permission" });
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] PermissionDto dto)
    {
        var permission = await _permissionService.CreatePermissionAsync(dto);
        return CreatedAtAction(nameof(GetAllPermissions), new { id = permission.Id }, permission);
    }
}
