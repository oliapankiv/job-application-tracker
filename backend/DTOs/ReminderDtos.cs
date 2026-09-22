namespace JobTracker.Api.DTOs;

public record ReminderDto(int Id, int ApplicationId, string CompanyName, string JobTitle, DateTime DueDate, string Message, bool IsCompleted);

public record CreateReminderDto(DateTime DueDate, string Message);

public record UpdateReminderDto(DateTime DueDate, string Message, bool IsCompleted);
