using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using JobTracker.Api.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using static JobTracker.Api.Tests.Helpers.TestDbContextFactory;

namespace JobTracker.Api.Tests.Services;

public class ApplicationServiceTests
{
    private static CreateApplicationDto NewCreateDto(DateTime? appliedDate = null) => new(
        "Acme", "Backend Engineer", "https://acme.test/jobs/1", ApplicationSource.LinkedIn,
        100_000m, "Kyiv", WorkType.Hybrid, appliedDate, "notes", "dotnet");

    private static UpdateApplicationDto NewUpdateDto() => new(
        "Globex", "Senior Engineer", null, ApplicationSource.Referral,
        120_000m, "Lviv", WorkType.Remote, new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc), "updated", "csharp");

    [Fact]
    public async Task CreateApplicationAsync_PersistsApplicationWithAppliedStatusAndInitialHistory()
    {
        using var db = Create();
        var sut = new ApplicationService(db);

        var result = await sut.CreateApplicationAsync(UserId, NewCreateDto());

        Assert.True(result.Id > 0);
        Assert.Equal(ApplicationStatus.Applied, result.Status);
        Assert.Equal("Acme", result.CompanyName);

        var saved = await db.Applications.Include(a => a.StatusHistories).SingleAsync();
        Assert.Equal(UserId, saved.UserId);
        var history = Assert.Single(saved.StatusHistories);
        Assert.Equal(ApplicationStatus.Applied, history.Status);
        Assert.Equal("Application created", history.Note);
    }

    [Fact]
    public async Task CreateApplicationAsync_UsesProvidedAppliedDate()
    {
        using var db = Create();
        var sut = new ApplicationService(db);
        var appliedDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = await sut.CreateApplicationAsync(UserId, NewCreateDto(appliedDate));

        Assert.Equal(appliedDate, result.AppliedDate);
    }

    [Fact]
    public async Task GetApplicationByIdAsync_ReturnsNull_ForOtherUsersApplication()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationByIdAsync(UserId, app.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetApplicationByIdAsync_ReturnsDetailsWithRelatedEntities()
    {
        var databaseName = Guid.NewGuid().ToString();
        var app = NewApplication();
        app.Contacts.Add(new Contact { Name = "Jane Recruiter" });
        app.Reminders.Add(new Reminder { Message = "Follow up", DueDate = DateTime.UtcNow.AddDays(3) });
        app.StatusHistories.Add(new StatusHistory { Status = ApplicationStatus.Applied, ChangedAt = DateTime.UtcNow.AddDays(-2) });
        app.StatusHistories.Add(new StatusHistory { Status = ApplicationStatus.Screening, ChangedAt = DateTime.UtcNow });
        using (var seedDb = Create(databaseName))
        {
            seedDb.Applications.Add(app);
            await seedDb.SaveChangesAsync();
        }

        using var db = Create(databaseName);
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationByIdAsync(UserId, app.Id);

        Assert.NotNull(result);
        Assert.Equal("Jane Recruiter", Assert.Single(result.Contacts).Name);
        var reminder = Assert.Single(result.Reminders);
        Assert.Equal(app.CompanyName, reminder.CompanyName);
        Assert.Equal(2, result.StatusHistory.Count);
        Assert.Equal(ApplicationStatus.Screening, result.StatusHistory[0].Status); // newest first
    }

    [Fact]
    public async Task GetApplicationsAsync_ReturnsOnlyCurrentUsersApplications()
    {
        using var db = Create();
        db.Applications.AddRange(
            NewApplication(companyName: "Mine"),
            NewApplication(userId: OtherUserId, companyName: "Theirs"));
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, null, 20);

        Assert.Equal("Mine", Assert.Single(result.Items).CompanyName);
        Assert.False(result.HasMore);
        Assert.Null(result.NextCursor);
    }

    [Fact]
    public async Task GetApplicationsAsync_FiltersByStatus()
    {
        using var db = Create();
        db.Applications.AddRange(
            NewApplication(companyName: "A", status: ApplicationStatus.Interview),
            NewApplication(companyName: "B", status: ApplicationStatus.Rejected));
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(UserId, ApplicationStatus.Interview, null, null, null, null, null, 20);

        Assert.Equal("A", Assert.Single(result.Items).CompanyName);
    }

    [Theory]
    [InlineData("acme")]
    [InlineData("  ACME ")]
    [InlineData("designer")]
    public async Task GetApplicationsAsync_SearchMatchesCompanyOrTitleCaseInsensitively(string search)
    {
        using var db = Create();
        db.Applications.AddRange(
            NewApplication(companyName: "Acme Corp", jobTitle: "Product Designer"),
            NewApplication(companyName: "Globex", jobTitle: "Engineer"));
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(UserId, null, search, null, null, null, null, 20);

        Assert.Equal("Acme Corp", Assert.Single(result.Items).CompanyName);
    }

    [Fact]
    public async Task GetApplicationsAsync_FiltersByTagAndDateRange()
    {
        using var db = Create();
        var jan = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        var mar = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        db.Applications.AddRange(
            NewApplication(companyName: "InRange", appliedDate: mar, tags: "dotnet,remote"),
            NewApplication(companyName: "TooEarly", appliedDate: jan, tags: "dotnet"),
            NewApplication(companyName: "WrongTag", appliedDate: mar, tags: "java"));
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(
            UserId, null, null, "dotnet", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc), null, null, 20);

        Assert.Equal("InRange", Assert.Single(result.Items).CompanyName);
    }

    [Fact]
    public async Task GetApplicationsAsync_PaginatesWithCursor_NewestFirstWithoutOverlap()
    {
        using var db = Create();
        var baseTime = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        for (var i = 0; i < 5; i++)
            db.Applications.Add(NewApplication(companyName: $"C{i}", lastUpdated: baseTime.AddHours(i)));
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var page1 = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, null, 2);
        var page2 = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, page1.NextCursor, 2);
        var page3 = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, page2.NextCursor, 2);

        Assert.Equal(["C4", "C3"], page1.Items.Select(a => a.CompanyName));
        Assert.True(page1.HasMore);
        Assert.NotNull(page1.NextCursor);
        Assert.Equal(["C2", "C1"], page2.Items.Select(a => a.CompanyName));
        Assert.Equal(["C0"], page3.Items.Select(a => a.CompanyName));
        Assert.False(page3.HasMore);
        Assert.Null(page3.NextCursor);
    }

    [Theory]
    [InlineData("not-base64!!")]
    [InlineData("Zm9v")]
    public async Task GetApplicationsAsync_IgnoresInvalidCursor(string cursor)
    {
        using var db = Create();
        db.Applications.AddRange(NewApplication(), NewApplication());
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, cursor, 20);

        Assert.Equal(2, result.Items.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(101)]
    public async Task GetApplicationsAsync_FallsBackToDefaultPageSize_WhenOutOfRange(int pageSize)
    {
        using var db = Create();
        for (var i = 0; i < 25; i++)
            db.Applications.Add(NewApplication());
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var result = await sut.GetApplicationsAsync(UserId, null, null, null, null, null, null, pageSize);

        Assert.Equal(20, result.Items.Count);
        Assert.True(result.HasMore);
    }

    [Fact]
    public async Task UpdateApplicationAsync_UpdatesFieldsAndLastUpdated()
    {
        using var db = Create();
        var app = NewApplication(lastUpdated: DateTime.UtcNow.AddDays(-10));
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var previousLastUpdated = app.LastUpdated;
        var sut = new ApplicationService(db);

        var updated = await sut.UpdateApplicationAsync(UserId, app.Id, NewUpdateDto());

        Assert.True(updated);
        var saved = await db.Applications.SingleAsync();
        Assert.Equal("Globex", saved.CompanyName);
        Assert.Equal(WorkType.Remote, saved.WorkType);
        Assert.True(saved.LastUpdated > previousLastUpdated);
    }

    [Fact]
    public async Task UpdateApplicationAsync_ReturnsFalse_ForOtherUsersApplication()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var updated = await sut.UpdateApplicationAsync(UserId, app.Id, NewUpdateDto());

        Assert.False(updated);
        Assert.Equal("Acme", (await db.Applications.SingleAsync()).CompanyName);
    }

    [Fact]
    public async Task DeleteApplicationAsync_RemovesOwnApplication()
    {
        using var db = Create();
        var app = NewApplication();
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var deleted = await sut.DeleteApplicationAsync(UserId, app.Id);

        Assert.True(deleted);
        Assert.Empty(db.Applications);
    }

    [Fact]
    public async Task DeleteApplicationAsync_ReturnsFalse_WhenNotFound()
    {
        using var db = Create();
        var sut = new ApplicationService(db);

        Assert.False(await sut.DeleteApplicationAsync(UserId, 999));
    }

    [Fact]
    public async Task UpdateStatusAsync_ChangesStatusAndAppendsHistory()
    {
        using var db = Create();
        var app = NewApplication();
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var updated = await sut.UpdateStatusAsync(UserId, app.Id, new UpdateStatusDto(ApplicationStatus.Interview, "Phone screen passed"));

        Assert.True(updated);
        Assert.Equal(ApplicationStatus.Interview, (await db.Applications.SingleAsync()).Status);
        var history = await db.StatusHistories.SingleAsync();
        Assert.Equal(ApplicationStatus.Interview, history.Status);
        Assert.Equal("Phone screen passed", history.Note);
        Assert.Equal(app.Id, history.ApplicationId);
    }

    [Fact]
    public async Task UpdateStatusAsync_ReturnsFalse_ForOtherUsersApplication()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var updated = await sut.UpdateStatusAsync(UserId, app.Id, new UpdateStatusDto(ApplicationStatus.Offer, null));

        Assert.False(updated);
        Assert.Empty(db.StatusHistories);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsNull_WhenApplicationNotOwned()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        Assert.Null(await sut.GetHistoryAsync(UserId, app.Id));
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsHistoryNewestFirst()
    {
        using var db = Create();
        var app = NewApplication();
        app.StatusHistories.Add(new StatusHistory { Status = ApplicationStatus.Applied, ChangedAt = DateTime.UtcNow.AddDays(-5) });
        app.StatusHistories.Add(new StatusHistory { Status = ApplicationStatus.Offer, ChangedAt = DateTime.UtcNow });
        app.StatusHistories.Add(new StatusHistory { Status = ApplicationStatus.Interview, ChangedAt = DateTime.UtcNow.AddDays(-1) });
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ApplicationService(db);

        var history = await sut.GetHistoryAsync(UserId, app.Id);

        Assert.NotNull(history);
        Assert.Equal(
            [ApplicationStatus.Offer, ApplicationStatus.Interview, ApplicationStatus.Applied],
            history.Select(h => h.Status));
    }
}
