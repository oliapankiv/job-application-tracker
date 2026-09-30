using JobTracker.Api.Controllers;
using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using JobTracker.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JobTracker.Api.Tests.Controllers;

public class RemindersControllerTests
{
    private const string UserId = "user-1";

    private readonly Mock<IReminderService> _service = new(MockBehavior.Strict);
    private readonly RemindersController _sut;

    public RemindersControllerTests()
    {
        _sut = new RemindersController(_service.Object).WithUser(UserId);
    }

    [Fact]
    public async Task GetUpcoming_ReturnsOkWithReminders()
    {
        List<ReminderDto> reminders = [new(1, 7, "Acme", "Engineer", DateTime.UtcNow, "Follow up", false)];
        _service.Setup(s => s.GetUpcomingAsync(UserId)).ReturnsAsync(reminders);

        var result = await _sut.GetUpcoming();

        Assert.Same(reminders, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task CreateReminder_ReturnsOk_WhenCreated()
    {
        var input = new CreateReminderDto(DateTime.UtcNow, "Follow up");
        var created = new ReminderDto(1, 7, "Acme", "Engineer", input.DueDate, input.Message, false);
        _service.Setup(s => s.CreateReminderAsync(UserId, 7, input)).ReturnsAsync(created);

        var result = await _sut.CreateReminder(7, input);

        Assert.Same(created, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task CreateReminder_ReturnsNotFound_WhenApplicationMissing()
    {
        var input = new CreateReminderDto(DateTime.UtcNow, "Follow up");
        _service.Setup(s => s.CreateReminderAsync(UserId, 7, input)).ReturnsAsync((ReminderDto?)null);

        var result = await _sut.CreateReminder(7, input);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task UpdateReminder_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        var input = new UpdateReminderDto(DateTime.UtcNow, "x", true);
        _service.Setup(s => s.UpdateReminderAsync(UserId, 1, input)).ReturnsAsync(serviceResult);

        Assert.IsType(expected, await _sut.UpdateReminder(1, input));
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task DeleteReminder_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        _service.Setup(s => s.DeleteReminderAsync(UserId, 1)).ReturnsAsync(serviceResult);

        Assert.IsType(expected, await _sut.DeleteReminder(1));
    }
}
