using JobTracker.Api.Controllers;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Services;
using JobTracker.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JobTracker.Api.Tests.Controllers;

public class ApplicationsControllerTests
{
    private const string UserId = "user-1";

    private readonly Mock<IApplicationService> _service = new(MockBehavior.Strict);
    private readonly ApplicationsController _sut;

    public ApplicationsControllerTests()
    {
        _sut = new ApplicationsController(_service.Object).WithUser(UserId);
    }

    private static ApplicationDto NewDto(int id = 1) => new(
        id, "Acme", "Engineer", null, ApplicationStatus.Applied, ApplicationSource.Other,
        null, null, WorkType.Remote, DateTime.UtcNow, DateTime.UtcNow, null, null);

    private static ApplicationDetailDto NewDetailDto(int id = 1) => new(
        id, "Acme", "Engineer", null, ApplicationStatus.Applied, ApplicationSource.Other,
        null, null, WorkType.Remote, DateTime.UtcNow, DateTime.UtcNow, null, null,
        [], [], [], []);

    private static UpdateApplicationDto NewUpdateDto() => new(
        "Acme", "Engineer", null, ApplicationSource.Other, null, null, WorkType.Remote, DateTime.UtcNow, null, null);

    [Fact]
    public async Task GetApplications_PassesQueryAndUserIdToService_AndReturnsOk()
    {
        var paged = new CursorPagedResult<ApplicationDto>([NewDto()], "next", true);
        var from = new DateTime(2026, 1, 1);
        _service
            .Setup(s => s.GetApplicationsAsync(UserId, ApplicationStatus.Interview, "acme", "dotnet", from, null, "cur", 10))
            .ReturnsAsync(paged);

        var result = await _sut.GetApplications(ApplicationStatus.Interview, "acme", "dotnet", from, null, "cur", 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(paged, ok.Value);
        _service.VerifyAll();
    }

    [Fact]
    public async Task GetApplication_ReturnsOk_WhenFound()
    {
        var detail = NewDetailDto(5);
        _service.Setup(s => s.GetApplicationByIdAsync(UserId, 5)).ReturnsAsync(detail);

        var result = await _sut.GetApplication(5);

        Assert.Same(detail, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task GetApplication_ReturnsNotFound_WhenMissing()
    {
        _service.Setup(s => s.GetApplicationByIdAsync(UserId, 5)).ReturnsAsync((ApplicationDetailDto?)null);

        var result = await _sut.GetApplication(5);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateApplication_ReturnsCreatedAtActionPointingToGetApplication()
    {
        var input = new CreateApplicationDto("Acme", "Engineer", null, ApplicationSource.Other, null, null, WorkType.Remote, null, null, null);
        var created = NewDto(42);
        _service.Setup(s => s.CreateApplicationAsync(UserId, input)).ReturnsAsync(created);

        var result = await _sut.CreateApplication(input);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ApplicationsController.GetApplication), createdResult.ActionName);
        Assert.Equal(42, createdResult.RouteValues!["id"]);
        Assert.Same(created, createdResult.Value);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task UpdateApplication_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        var dto = NewUpdateDto();
        _service.Setup(s => s.UpdateApplicationAsync(UserId, 3, dto)).ReturnsAsync(serviceResult);

        var result = await _sut.UpdateApplication(3, dto);

        Assert.IsType(expected, result);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task DeleteApplication_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        _service.Setup(s => s.DeleteApplicationAsync(UserId, 3)).ReturnsAsync(serviceResult);

        var result = await _sut.DeleteApplication(3);

        Assert.IsType(expected, result);
        _service.Verify(s => s.DeleteApplicationAsync(UserId, 3), Times.Once);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task UpdateStatus_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        var dto = new UpdateStatusDto(ApplicationStatus.Offer, "yay");
        _service.Setup(s => s.UpdateStatusAsync(UserId, 3, dto)).ReturnsAsync(serviceResult);

        var result = await _sut.UpdateStatus(3, dto);

        Assert.IsType(expected, result);
    }

    [Fact]
    public async Task GetHistory_ReturnsNotFound_WhenServiceReturnsNull()
    {
        _service.Setup(s => s.GetHistoryAsync(UserId, 3)).ReturnsAsync((List<StatusHistoryDto>?)null);

        var result = await _sut.GetHistory(3);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetHistory_ReturnsOk_WithHistory()
    {
        List<StatusHistoryDto> history = [new(1, ApplicationStatus.Applied, DateTime.UtcNow, null)];
        _service.Setup(s => s.GetHistoryAsync(UserId, 3)).ReturnsAsync(history);

        var result = await _sut.GetHistory(3);

        Assert.Same(history, Assert.IsType<OkObjectResult>(result.Result).Value);
    }
}
