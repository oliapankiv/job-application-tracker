using JobTracker.Api.Models;
using JobTracker.Api.Services;
using static JobTracker.Api.Tests.Helpers.TestDbContextFactory;

namespace JobTracker.Api.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetStatsAsync_ReturnsZeroes_WhenUserHasNoApplications()
    {
        using var db = Create();
        var sut = new DashboardService(db);

        var stats = await sut.GetStatsAsync(UserId);

        Assert.Equal(0, stats.TotalApplications);
        Assert.Equal(0, stats.ResponseRate);
        Assert.All(stats.StatusBreakdown, s => Assert.Equal(0, s.Count));
        Assert.Equal(Enum.GetValues<ApplicationStatus>().Length, stats.StatusBreakdown.Count);
        Assert.Equal(12, stats.ApplicationsPerWeek.Count);
    }

    [Fact]
    public async Task GetStatsAsync_ComputesCountsAndResponseRate()
    {
        using var db = Create();
        var now = DateTime.UtcNow;
        db.Applications.AddRange(
            NewApplication(status: ApplicationStatus.Applied, appliedDate: now),
            NewApplication(status: ApplicationStatus.Interview, appliedDate: now),
            NewApplication(status: ApplicationStatus.Offer, appliedDate: now),
            NewApplication(status: ApplicationStatus.Rejected, appliedDate: now.AddYears(-1)),
            NewApplication(userId: OtherUserId, status: ApplicationStatus.Offer, appliedDate: now));
        await db.SaveChangesAsync();
        var sut = new DashboardService(db);

        var stats = await sut.GetStatsAsync(UserId);

        Assert.Equal(4, stats.TotalApplications);
        Assert.Equal(3, stats.ApplicationsThisMonth);
        Assert.Equal(3, stats.ApplicationsThisWeek);
        Assert.Equal(75.0, stats.ResponseRate); // 3 of 4 moved past Applied
        Assert.Equal(1, stats.ActiveInterviews);
        Assert.Equal(1, stats.OffersReceived);
        Assert.Equal(1, stats.StatusBreakdown.Single(s => s.Status == nameof(ApplicationStatus.Rejected)).Count);
    }

    [Fact]
    public async Task GetStatsAsync_RoundsResponseRateToOneDecimal()
    {
        using var db = Create();
        db.Applications.AddRange(
            NewApplication(status: ApplicationStatus.Screening),
            NewApplication(status: ApplicationStatus.Applied),
            NewApplication(status: ApplicationStatus.Applied));
        await db.SaveChangesAsync();
        var sut = new DashboardService(db);

        var stats = await sut.GetStatsAsync(UserId);

        Assert.Equal(33.3, stats.ResponseRate);
    }

    [Fact]
    public async Task GetStatsAsync_BucketsApplicationsPerWeek_EndingWithCurrentWeek()
    {
        using var db = Create();
        var now = DateTime.UtcNow;
        db.Applications.AddRange(
            NewApplication(appliedDate: now),
            NewApplication(appliedDate: now),
            NewApplication(appliedDate: now.AddDays(-7)),
            NewApplication(appliedDate: now.AddDays(-7 * 20))); // outside the 12-week window
        await db.SaveChangesAsync();
        var sut = new DashboardService(db);

        var stats = await sut.GetStatsAsync(UserId);

        var weeks = stats.ApplicationsPerWeek;
        var currentWeekStart = now.Date.AddDays(-(int)now.DayOfWeek).ToString("yyyy-MM-dd");
        Assert.Equal(12, weeks.Count);
        Assert.Equal(currentWeekStart, weeks[^1].WeekStart);
        Assert.Equal(2, weeks[^1].Count);
        Assert.Equal(1, weeks[^2].Count);
        Assert.Equal(3, weeks.Sum(w => w.Count));
    }
}
