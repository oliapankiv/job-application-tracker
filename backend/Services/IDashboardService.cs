using JobTracker.Api.DTOs;

namespace JobTracker.Api.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync(string userId);
}
