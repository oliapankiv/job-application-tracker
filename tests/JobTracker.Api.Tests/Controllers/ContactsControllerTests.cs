using JobTracker.Api.Controllers;
using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using JobTracker.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JobTracker.Api.Tests.Controllers;

public class ContactsControllerTests
{
    private const string UserId = "user-1";

    private readonly Mock<IContactService> _service = new(MockBehavior.Strict);
    private readonly ContactsController _sut;
    private readonly CreateContactDto _input = new("Jane", null, null, null, null);

    public ContactsControllerTests()
    {
        _sut = new ContactsController(_service.Object).WithUser(UserId);
    }

    [Fact]
    public async Task GetContacts_ReturnsOkWithContacts()
    {
        List<ContactDto> contacts = [new(1, 7, "Jane", null, null, null, null)];
        _service.Setup(s => s.GetContactsAsync(UserId, 7)).ReturnsAsync(contacts);

        var result = await _sut.GetContacts(7);

        Assert.Same(contacts, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task CreateContact_ReturnsOk_WhenCreated()
    {
        var created = new ContactDto(1, 7, "Jane", null, null, null, null);
        _service.Setup(s => s.CreateContactAsync(UserId, 7, _input)).ReturnsAsync(created);

        var result = await _sut.CreateContact(7, _input);

        Assert.Same(created, Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public async Task CreateContact_ReturnsNotFound_WhenApplicationMissing()
    {
        _service.Setup(s => s.CreateContactAsync(UserId, 7, _input)).ReturnsAsync((ContactDto?)null);

        var result = await _sut.CreateContact(7, _input);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task UpdateContact_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        _service.Setup(s => s.UpdateContactAsync(UserId, 1, _input)).ReturnsAsync(serviceResult);

        Assert.IsType(expected, await _sut.UpdateContact(1, _input));
    }

    [Theory]
    [InlineData(true, typeof(NoContentResult))]
    [InlineData(false, typeof(NotFoundResult))]
    public async Task DeleteContact_MapsServiceResultToStatusCode(bool serviceResult, Type expected)
    {
        _service.Setup(s => s.DeleteContactAsync(UserId, 1)).ReturnsAsync(serviceResult);

        Assert.IsType(expected, await _sut.DeleteContact(1));
    }
}
