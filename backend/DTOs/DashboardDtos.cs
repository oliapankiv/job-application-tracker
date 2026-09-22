namespace JobTracker.Api.DTOs;

public record DashboardStatsDto(
    int TotalApplications,
    int ApplicationsThisMonth,
    int ApplicationsThisWeek,
    double ResponseRate,
    int ActiveInterviews,
    int OffersReceived,
    List<StatusCountDto> StatusBreakdown,
    List<WeeklyCountDto> ApplicationsPerWeek
);

public record StatusCountDto(string Status, int Count);

public record WeeklyCountDto(string WeekStart, int Count);
