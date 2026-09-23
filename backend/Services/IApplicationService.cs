using JobTracker.Api.DTOs;
using JobTracker.Api.Models;

namespace JobTracker.Api.Services;

public interface IApplicationService
{
    Task<CursorPagedResult<ApplicationDto>> GetApplicationsAsync(
        string userId, ApplicationStatus? status, string? search, string? tag,
        DateTime? fromDate, DateTime? toDate, string? cursor, int pageSize);

    Task<ApplicationDetailDto?> GetApplicationByIdAsync(string userId, int id);

    Task<ApplicationDto> CreateApplicationAsync(string userId, CreateApplicationDto dto);

    Task<bool> UpdateApplicationAsync(string userId, int id, UpdateApplicationDto dto);

    Task<bool> DeleteApplicationAsync(string userId, int id);

    Task<bool> UpdateStatusAsync(string userId, int id, UpdateStatusDto dto);

    Task<List<StatusHistoryDto>?> GetHistoryAsync(string userId, int id);
}
