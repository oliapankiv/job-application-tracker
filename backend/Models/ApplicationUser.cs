using Microsoft.AspNetCore.Identity;

namespace JobTracker.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
