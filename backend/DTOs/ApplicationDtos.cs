using JobTracker.Api.Models;

namespace JobTracker.Api.DTOs;

public record ApplicationDto(
    int Id,
    string CompanyName,
    string JobTitle,
    string? JobPostingUrl,
    ApplicationStatus Status,
    ApplicationSource Source,
    decimal? Salary,
    string? Location,
    WorkType WorkType,
    DateTime AppliedDate,
    DateTime LastUpdated,
    string? Notes,
    string? Tags
);

public record ApplicationDetailDto(
    int Id,
    string CompanyName,
    string JobTitle,
    string? JobPostingUrl,
    ApplicationStatus Status,
    ApplicationSource Source,
    decimal? Salary,
    string? Location,
    WorkType WorkType,
    DateTime AppliedDate,
    DateTime LastUpdated,
    string? Notes,
    string? Tags,
    List<StatusHistoryDto> StatusHistory,
    List<ContactDto> Contacts,
    List<ReminderDto> Reminders,
    List<DocumentDto> Documents
);

public record CreateApplicationDto(
    string CompanyName,
    string JobTitle,
    string? JobPostingUrl,
    ApplicationSource Source,
    decimal? Salary,
    string? Location,
    WorkType WorkType,
    DateTime? AppliedDate,
    string? Notes,
    string? Tags
);

public record UpdateApplicationDto(
    string CompanyName,
    string JobTitle,
    string? JobPostingUrl,
    ApplicationSource Source,
    decimal? Salary,
    string? Location,
    WorkType WorkType,
    DateTime AppliedDate,
    string? Notes,
    string? Tags
);

public record UpdateStatusDto(ApplicationStatus Status, string? Note);

public record StatusHistoryDto(int Id, ApplicationStatus Status, DateTime ChangedAt, string? Note);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
