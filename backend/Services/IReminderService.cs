using JobTracker.Api.DTOs;

namespace JobTracker.Api.Services;

public interface IReminderService
{
    Task<ReminderDto?> CreateReminderAsync(string userId, int applicationId, CreateReminderDto dto);
    Task<List<ReminderDto>> GetUpcomingAsync(string userId);
    Task<bool> UpdateReminderAsync(string userId, int reminderId, UpdateReminderDto dto);
    Task<bool> DeleteReminderAsync(string userId, int reminderId);
}
