using JobTracker.Api.Data;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardStatsDto> GetStatsAsync(string userId)
    {
        var apps = await db.Applications
            .Where(a => a.UserId == userId)
            .Select(a => new { a.Status, a.AppliedDate })
            .ToListAsync();

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);

        var totalApplications = apps.Count;
        var applicationsThisMonth = apps.Count(a => a.AppliedDate >= startOfMonth);
        var applicationsThisWeek = apps.Count(a => a.AppliedDate >= startOfWeek);

        var respondedCount = apps.Count(a => a.Status != ApplicationStatus.Applied);
        var responseRate = totalApplications == 0 ? 0 : Math.Round(respondedCount * 100.0 / totalApplications, 1);

        var activeInterviews = apps.Count(a => a.Status == ApplicationStatus.Interview);
        var offersReceived = apps.Count(a => a.Status == ApplicationStatus.Offer);

        var statusBreakdown = Enum.GetValues<ApplicationStatus>()
            .Select(s => new StatusCountDto(s.ToString(), apps.Count(a => a.Status == s)))
            .ToList();

        var twelveWeeksAgo = now.Date.AddDays(-7 * 11 - (int)now.DayOfWeek);
        var weeklyBuckets = new List<WeeklyCountDto>();
        for (var weekStart = twelveWeeksAgo; weekStart <= now.Date; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(7);
            var count = apps.Count(a => a.AppliedDate >= weekStart && a.AppliedDate < weekEnd);
            weeklyBuckets.Add(new WeeklyCountDto(weekStart.ToString("yyyy-MM-dd"), count));
        }

        return new DashboardStatsDto(
            totalApplications,
            applicationsThisMonth,
            applicationsThisWeek,
            responseRate,
            activeInterviews,
            offersReceived,
            statusBreakdown,
            weeklyBuckets
        );
    }
}
