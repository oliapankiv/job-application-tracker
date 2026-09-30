using JobTracker.Api.Controllers;
using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using JobTracker.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JobTracker.Api.Tests.Controllers;

public class DashboardControllerTests
{
    [Fact]
    public async Task GetStats_ReturnsStatsForAuthenticatedUser()
    {
        var stats = new DashboardStatsDto(3, 2, 1, 66.7, 1, 0, [], []);
        var service = new Mock<IDashboardService>();
        service.Setup(s => s.GetStatsAsync("user-1")).ReturnsAsync(stats);
        var sut = new DashboardController(service.Object).WithUser("user-1");

        var result = await sut.GetStats();

        Assert.Same(stats, Assert.IsType<OkObjectResult>(result.Result).Value);
        service.Verify(s => s.GetStatsAsync("user-1"), Times.Once);
    }
}
