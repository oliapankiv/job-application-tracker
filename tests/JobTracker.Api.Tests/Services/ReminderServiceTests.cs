using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using static JobTracker.Api.Tests.Helpers.TestDbContextFactory;

namespace JobTracker.Api.Tests.Services;

public class ReminderServiceTests
{
    [Fact]
    public async Task CreateReminderAsync_AddsIncompleteReminderWithApplicationInfo()
    {
        using var db = Create();
        var app = NewApplication(companyName: "Acme", jobTitle: "Engineer");
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);
        var due = DateTime.UtcNow.AddDays(2);

        var result = await sut.CreateReminderAsync(UserId, app.Id, new CreateReminderDto(due, "Follow up"));

        Assert.NotNull(result);
        Assert.Equal("Acme", result.CompanyName);
        Assert.Equal("Engineer", result.JobTitle);
        Assert.Equal(due, result.DueDate);
        Assert.False(result.IsCompleted);
        Assert.Single(db.Reminders);
    }

    [Fact]
    public async Task CreateReminderAsync_ReturnsNull_WhenApplicationNotOwned()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);

        var result = await sut.CreateReminderAsync(UserId, app.Id, new CreateReminderDto(DateTime.UtcNow, "Follow up"));

        Assert.Null(result);
        Assert.Empty(db.Reminders);
    }

    [Fact]
    public async Task GetUpcomingAsync_ReturnsOwnIncompleteRemindersOrderedByDueDate()
    {
        using var db = Create();
        var now = DateTime.UtcNow;
        var mine = NewApplication();
        mine.Reminders.Add(new Reminder { Message = "later", DueDate = now.AddDays(5) });
        mine.Reminders.Add(new Reminder { Message = "sooner", DueDate = now.AddDays(1) });
        mine.Reminders.Add(new Reminder { Message = "done", DueDate = now, IsCompleted = true });
        var theirs = NewApplication(userId: OtherUserId);
        theirs.Reminders.Add(new Reminder { Message = "not mine", DueDate = now });
        db.Applications.AddRange(mine, theirs);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);

        var result = await sut.GetUpcomingAsync(UserId);

        Assert.Equal(["sooner", "later"], result.Select(r => r.Message));
    }

    [Fact]
    public async Task UpdateReminderAsync_UpdatesFields()
    {
        using var db = Create();
        var app = NewApplication();
        var reminder = new Reminder { Message = "old", DueDate = DateTime.UtcNow };
        app.Reminders.Add(reminder);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);
        var newDue = DateTime.UtcNow.AddDays(7);

        var updated = await sut.UpdateReminderAsync(UserId, reminder.Id, new UpdateReminderDto(newDue, "new", true));

        Assert.True(updated);
        var saved = await db.Reminders.SingleAsync();
        Assert.Equal("new", saved.Message);
        Assert.Equal(newDue, saved.DueDate);
        Assert.True(saved.IsCompleted);
    }

    [Fact]
    public async Task UpdateReminderAsync_ReturnsFalse_ForOtherUsersReminder()
    {
        using var db = Create();
        var app = NewApplication(userId: OtherUserId);
        var reminder = new Reminder { Message = "old", DueDate = DateTime.UtcNow };
        app.Reminders.Add(reminder);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);

        var updated = await sut.UpdateReminderAsync(UserId, reminder.Id, new UpdateReminderDto(DateTime.UtcNow, "new", true));

        Assert.False(updated);
        Assert.Equal("old", (await db.Reminders.SingleAsync()).Message);
    }

    [Fact]
    public async Task DeleteReminderAsync_RemovesOwnReminder()
    {
        using var db = Create();
        var app = NewApplication();
        var reminder = new Reminder { Message = "x", DueDate = DateTime.UtcNow };
        app.Reminders.Add(reminder);
        db.Applications.Add(app);
        await db.SaveChangesAsync();
        var sut = new ReminderService(db);

        Assert.True(await sut.DeleteReminderAsync(UserId, reminder.Id));
        Assert.Empty(db.Reminders);
    }

    [Fact]
    public async Task DeleteReminderAsync_ReturnsFalse_WhenNotFound()
    {
        using var db = Create();
        var sut = new ReminderService(db);

        Assert.False(await sut.DeleteReminderAsync(UserId, 42));
    }
}
