namespace JobTracker.Api.Models;

public class JobApplication
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? JobPostingUrl { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public ApplicationSource Source { get; set; } = ApplicationSource.Other;
    public decimal? Salary { get; set; }
    public string? Location { get; set; }
    public WorkType WorkType { get; set; } = WorkType.Remote;
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string? Tags { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
