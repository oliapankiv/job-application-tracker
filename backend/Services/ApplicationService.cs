using System.Text;
using JobTracker.Api.Data;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services;

public class ApplicationService(AppDbContext db) : IApplicationService
{
    public async Task<CursorPagedResult<ApplicationDto>> GetApplicationsAsync(
        string userId, ApplicationStatus? status, string? search, string? tag,
        DateTime? fromDate, DateTime? toDate, string? cursor, int pageSize)
    {
        var query = db.Applications.Where(a => a.UserId == userId);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                a.CompanyName.ToLower().Contains(term) ||
                a.JobTitle.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(a => a.Tags != null && a.Tags.Contains(tag));

        if (fromDate.HasValue)
            query = query.Where(a => a.AppliedDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.AppliedDate <= toDate.Value);

        pageSize = pageSize is <= 0 or > 100 ? 20 : pageSize;

        var cursorValue = DecodeCursor(cursor);
        if (cursorValue.HasValue)
        {
            var (cursorLastUpdated, cursorId) = cursorValue.Value;
            query = query.Where(a =>
                a.LastUpdated < cursorLastUpdated ||
                (a.LastUpdated == cursorLastUpdated && a.Id < cursorId));
        }

        var items = await query
            .OrderByDescending(a => a.LastUpdated)
            .ThenByDescending(a => a.Id)
            .Take(pageSize + 1)
            .Select(a => ToDto(a))
            .ToListAsync();

        var hasMore = items.Count > pageSize;
        if (hasMore)
            items.RemoveAt(items.Count - 1);

        var nextCursor = hasMore ? EncodeCursor(items[^1].LastUpdated, items[^1].Id) : null;

        return new CursorPagedResult<ApplicationDto>(items, nextCursor, hasMore);
    }

    private static string EncodeCursor(DateTime lastUpdated, int id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{lastUpdated.Ticks}:{id}"));

    private static (DateTime LastUpdated, int Id)? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor)) return null;

        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Split(':');
            if (parts.Length != 2) return null;

            var ticks = long.Parse(parts[0]);
            var id = int.Parse(parts[1]);
            return (new DateTime(ticks, DateTimeKind.Utc), id);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (OverflowException)
        {
            return null;
        }
    }

    public async Task<ApplicationDetailDto?> GetApplicationByIdAsync(string userId, int id)
    {
        var app = await db.Applications
            .Include(a => a.StatusHistories.OrderByDescending(h => h.ChangedAt))
            .Include(a => a.Contacts)
            .Include(a => a.Reminders)
            .Include(a => a.Documents)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (app is null) return null;

        return new ApplicationDetailDto(
            app.Id, app.CompanyName, app.JobTitle, app.JobPostingUrl, app.Status, app.Source,
            app.Salary, app.Location, app.WorkType, app.AppliedDate, app.LastUpdated, app.Notes, app.Tags,
            app.StatusHistories.Select(h => new StatusHistoryDto(h.Id, h.Status, h.ChangedAt, h.Note)).ToList(),
            app.Contacts.Select(c => new ContactDto(c.Id, c.ApplicationId, c.Name, c.Role, c.Email, c.Phone, c.LinkedInUrl)).ToList(),
            app.Reminders.Select(r => new ReminderDto(r.Id, r.ApplicationId, app.CompanyName, app.JobTitle, r.DueDate, r.Message, r.IsCompleted)).ToList(),
            app.Documents.Select(d => new DocumentDto(d.Id, d.ApplicationId, d.FileName, d.FilePath, d.DocumentType, d.UploadedAt)).ToList()
        );
    }

    public async Task<ApplicationDto> CreateApplicationAsync(string userId, CreateApplicationDto dto)
    {
        var now = DateTime.UtcNow;
        var app = new JobApplication
        {
            UserId = userId,
            CompanyName = dto.CompanyName,
            JobTitle = dto.JobTitle,
            JobPostingUrl = dto.JobPostingUrl,
            Source = dto.Source,
            Salary = dto.Salary,
            Location = dto.Location,
            WorkType = dto.WorkType,
            AppliedDate = dto.AppliedDate ?? now,
            LastUpdated = now,
            Notes = dto.Notes,
            Tags = dto.Tags,
            Status = ApplicationStatus.Applied
        };

        app.StatusHistories.Add(new StatusHistory
        {
            Status = ApplicationStatus.Applied,
            ChangedAt = now,
            Note = "Application created"
        });

        db.Applications.Add(app);
        await db.SaveChangesAsync();

        return ToDto(app);
    }

    public async Task<bool> UpdateApplicationAsync(string userId, int id, UpdateApplicationDto dto)
    {
        var app = await db.Applications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (app is null) return false;

        app.CompanyName = dto.CompanyName;
        app.JobTitle = dto.JobTitle;
        app.JobPostingUrl = dto.JobPostingUrl;
        app.Source = dto.Source;
        app.Salary = dto.Salary;
        app.Location = dto.Location;
        app.WorkType = dto.WorkType;
        app.AppliedDate = dto.AppliedDate;
        app.Notes = dto.Notes;
        app.Tags = dto.Tags;
        app.LastUpdated = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteApplicationAsync(string userId, int id)
    {
        var app = await db.Applications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (app is null) return false;

        db.Applications.Remove(app);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(string userId, int id, UpdateStatusDto dto)
    {
        var app = await db.Applications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (app is null) return false;

        app.Status = dto.Status;
        app.LastUpdated = DateTime.UtcNow;

        db.StatusHistories.Add(new StatusHistory
        {
            ApplicationId = app.Id,
            Status = dto.Status,
            ChangedAt = app.LastUpdated,
            Note = dto.Note
        });

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<StatusHistoryDto>?> GetHistoryAsync(string userId, int id)
    {
        var exists = await db.Applications.AnyAsync(a => a.Id == id && a.UserId == userId);
        if (!exists) return null;

        return await db.StatusHistories
            .Where(h => h.ApplicationId == id)
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new StatusHistoryDto(h.Id, h.Status, h.ChangedAt, h.Note))
            .ToListAsync();
    }

    private static ApplicationDto ToDto(JobApplication a) => new(
        a.Id, a.CompanyName, a.JobTitle, a.JobPostingUrl, a.Status, a.Source,
        a.Salary, a.Location, a.WorkType, a.AppliedDate, a.LastUpdated, a.Notes, a.Tags);
}
