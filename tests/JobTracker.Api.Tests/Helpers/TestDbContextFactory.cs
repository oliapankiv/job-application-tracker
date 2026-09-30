using JobTracker.Api.Data;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Tests.Helpers;

public static class TestDbContextFactory
{
    public const string UserId = "user-1";
    public const string OtherUserId = "user-2";

    // Each call gets its own isolated in-memory database.
    public static AppDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static JobApplication NewApplication(
        string userId = UserId,
        string companyName = "Acme",
        string jobTitle = "Engineer",
        ApplicationStatus status = ApplicationStatus.Applied,
        DateTime? appliedDate = null,
        DateTime? lastUpdated = null,
        string? tags = null) => new()
    {
        UserId = userId,
        CompanyName = companyName,
        JobTitle = jobTitle,
        Status = status,
        AppliedDate = appliedDate ?? DateTime.UtcNow,
        LastUpdated = lastUpdated ?? DateTime.UtcNow,
        Tags = tags
    };
}
