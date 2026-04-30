using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Tenant.Models;
using Modules.Tenant.Services;

namespace Bff.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TenantDto>>> GetAllTenants()
    {
        var tenants = await _tenantService.GetAllTenantsAsync();
        return Ok(tenants);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(id);
        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });
        return Ok(tenant);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<TenantDto>> GetTenantByCode(string code)
    {
        var tenant = await _tenantService.GetTenantByCodeAsync(code);
        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });
        return Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] CreateTenantRequest request)
    {
        var tenant = await _tenantService.CreateTenantAsync(request);
        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantDto>> UpdateTenant(Guid id, [FromBody] UpdateTenantRequest request)
    {
        var tenant = await _tenantService.UpdateTenantAsync(id, request);
        if (tenant == null)
            return NotFound(new { message = "Tenant not found" });
        return Ok(tenant);
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<ActionResult> SuspendTenant(Guid id)
    {
        var result = await _tenantService.SuspendTenantAsync(id);
        if (!result)
            return BadRequest(new { message = "Failed to suspend tenant" });
        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult> ActivateTenant(Guid id)
    {
        var result = await _tenantService.ActivateTenantAsync(id);
        if (!result)
            return BadRequest(new { message = "Failed to activate tenant" });
        return NoContent();
    }

    [HttpGet("{id:guid}/user-count")]
    public async Task<ActionResult<int>> GetUserCount(Guid id)
    {
        var count = await _tenantService.GetUserCountAsync(id);
        return Ok(count);
    }
}
