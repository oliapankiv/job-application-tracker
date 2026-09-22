namespace JobTracker.Api.Models;

public class StatusHistory
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public JobApplication? Application { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
}
