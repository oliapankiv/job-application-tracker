namespace JobTracker.Api.Models;

public class Reminder
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public JobApplication? Application { get; set; }
    public DateTime DueDate { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
