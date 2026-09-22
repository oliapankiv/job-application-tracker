using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Route("api")]
public class RemindersController(IReminderService reminderService) : ApiControllerBase
{
    [HttpGet("reminders/upcoming")]
    public async Task<ActionResult<List<ReminderDto>>> GetUpcoming()
    {
        var reminders = await reminderService.GetUpcomingAsync(UserId);
        return Ok(reminders);
    }

    [HttpPost("applications/{applicationId:int}/reminders")]
    public async Task<ActionResult<ReminderDto>> CreateReminder(int applicationId, CreateReminderDto dto)
    {
        var reminder = await reminderService.CreateReminderAsync(UserId, applicationId, dto);
        return reminder is null ? NotFound() : Ok(reminder);
    }

    [HttpPut("reminders/{reminderId:int}")]
    public async Task<IActionResult> UpdateReminder(int reminderId, UpdateReminderDto dto)
    {
        var updated = await reminderService.UpdateReminderAsync(UserId, reminderId, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("reminders/{reminderId:int}")]
    public async Task<IActionResult> DeleteReminder(int reminderId)
    {
        var deleted = await reminderService.DeleteReminderAsync(UserId, reminderId);
        return deleted ? NoContent() : NotFound();
    }
}
