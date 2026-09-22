using JobTracker.Api.DTOs;

namespace JobTracker.Api.Services;

public interface IContactService
{
    Task<ContactDto?> CreateContactAsync(string userId, int applicationId, CreateContactDto dto);
    Task<List<ContactDto>> GetContactsAsync(string userId, int applicationId);
    Task<bool> UpdateContactAsync(string userId, int contactId, CreateContactDto dto);
    Task<bool> DeleteContactAsync(string userId, int contactId);
}
