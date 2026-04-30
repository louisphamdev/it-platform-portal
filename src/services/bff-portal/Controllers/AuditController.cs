using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Audit.Models;
using Modules.Audit.Services;

namespace Bff.Portal.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpPost]
    public async Task<ActionResult> Log([FromBody] AuditLogDto logDto)
    {
        await _auditService.LogAsync(logDto);
        return Accepted();
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditLogDto>>> Query([FromQuery] AuditQueryRequest request)
    {
        var logs = await _auditService.QueryAsync(request);
        return Ok(logs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuditLogDto>> GetById(Guid id)
    {
        var log = await _auditService.GetByIdAsync(id);
        if (log == null)
            return NotFound(new { message = "Audit log not found" });
        return Ok(log);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetTotalCount([FromQuery] AuditQueryRequest request)
    {
        var count = await _auditService.GetTotalCountAsync(request);
        return Ok(count);
    }
}
