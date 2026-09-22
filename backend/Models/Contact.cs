namespace JobTracker.Api.Models;

public class Contact
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public JobApplication? Application { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
}
