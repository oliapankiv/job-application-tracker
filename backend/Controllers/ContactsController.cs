using JobTracker.Api.DTOs;
using JobTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Route("api")]
public class ContactsController(IContactService contactService) : ApiControllerBase
{
    [HttpGet("applications/{applicationId:int}/contacts")]
    public async Task<ActionResult<List<ContactDto>>> GetContacts(int applicationId)
    {
        var contacts = await contactService.GetContactsAsync(UserId, applicationId);
        return Ok(contacts);
    }

    [HttpPost("applications/{applicationId:int}/contacts")]
    public async Task<ActionResult<ContactDto>> CreateContact(int applicationId, CreateContactDto dto)
    {
        var contact = await contactService.CreateContactAsync(UserId, applicationId, dto);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpPut("contacts/{contactId:int}")]
    public async Task<IActionResult> UpdateContact(int contactId, CreateContactDto dto)
    {
        var updated = await contactService.UpdateContactAsync(UserId, contactId, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("contacts/{contactId:int}")]
    public async Task<IActionResult> DeleteContact(int contactId)
    {
        var deleted = await contactService.DeleteContactAsync(UserId, contactId);
        return deleted ? NoContent() : NotFound();
    }
}
