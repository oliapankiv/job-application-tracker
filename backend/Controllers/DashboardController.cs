using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboardService) : ApiControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var stats = await dashboardService.GetStatsAsync(UserId);
        return Ok(stats);
    }
}
