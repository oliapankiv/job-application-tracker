using JobTracker.Api.Data;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services;

public class ReminderService(AppDbContext db) : IReminderService
{
    public async Task<ReminderDto?> CreateReminderAsync(string userId, int applicationId, CreateReminderDto dto)
    {
        var app = await db.Applications.FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId);
        if (app is null) return null;

        var reminder = new Reminder
        {
            ApplicationId = applicationId,
            DueDate = dto.DueDate,
            Message = dto.Message,
            IsCompleted = false
        };

        db.Reminders.Add(reminder);
        await db.SaveChangesAsync();

        return new ReminderDto(reminder.Id, applicationId, app.CompanyName, app.JobTitle, reminder.DueDate, reminder.Message, reminder.IsCompleted);
    }

    public async Task<List<ReminderDto>> GetUpcomingAsync(string userId)
    {
        return await db.Reminders
            .Include(r => r.Application)
            .Where(r => r.Application!.UserId == userId && !r.IsCompleted)
            .OrderBy(r => r.DueDate)
            .Select(r => new ReminderDto(r.Id, r.ApplicationId, r.Application!.CompanyName, r.Application!.JobTitle, r.DueDate, r.Message, r.IsCompleted))
            .ToListAsync();
    }

    public async Task<bool> UpdateReminderAsync(string userId, int reminderId, UpdateReminderDto dto)
    {
        var reminder = await db.Reminders
            .Include(r => r.Application)
            .FirstOrDefaultAsync(r => r.Id == reminderId && r.Application!.UserId == userId);

        if (reminder is null) return false;

        reminder.DueDate = dto.DueDate;
        reminder.Message = dto.Message;
        reminder.IsCompleted = dto.IsCompleted;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReminderAsync(string userId, int reminderId)
    {
        var reminder = await db.Reminders
            .Include(r => r.Application)
            .FirstOrDefaultAsync(r => r.Id == reminderId && r.Application!.UserId == userId);

        if (reminder is null) return false;

        db.Reminders.Remove(reminder);
        await db.SaveChangesAsync();
        return true;
    }
}
