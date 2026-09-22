using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Route("api/applications")]
public class ApplicationsController(IApplicationService applicationService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ApplicationDto>>> GetApplications(
        [FromQuery] ApplicationStatus? status,
        [FromQuery] string? search,
        [FromQuery] string? tag,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await applicationService.GetApplicationsAsync(UserId, status, search, tag, fromDate, toDate, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApplicationDetailDto>> GetApplication(int id)
    {
        var app = await applicationService.GetApplicationByIdAsync(UserId, id);
        return app is null ? NotFound() : Ok(app);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication(CreateApplicationDto dto)
    {
        var created = await applicationService.CreateApplicationAsync(UserId, dto);
        return CreatedAtAction(nameof(GetApplication), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateApplication(int id, UpdateApplicationDto dto)
    {
        var updated = await applicationService.UpdateApplicationAsync(UserId, id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var deleted = await applicationService.DeleteApplicationAsync(UserId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
    {
        var updated = await applicationService.UpdateStatusAsync(UserId, id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<List<StatusHistoryDto>>> GetHistory(int id)
    {
        var history = await applicationService.GetHistoryAsync(UserId, id);
        return history is null ? NotFound() : Ok(history);
    }
}
