using JobTracker.Api.Data;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services;

public class ContactService(AppDbContext db) : IContactService
{
    public async Task<ContactDto?> CreateContactAsync(string userId, int applicationId, CreateContactDto dto)
    {
        var appExists = await db.Applications.AnyAsync(a => a.Id == applicationId && a.UserId == userId);
        if (!appExists) return null;

        var contact = new Contact
        {
            ApplicationId = applicationId,
            Name = dto.Name,
            Role = dto.Role,
            Email = dto.Email,
            Phone = dto.Phone,
            LinkedInUrl = dto.LinkedInUrl
        };

        db.Contacts.Add(contact);
        await db.SaveChangesAsync();

        return new ContactDto(contact.Id, applicationId, contact.Name, contact.Role, contact.Email, contact.Phone, contact.LinkedInUrl);
    }

    public async Task<List<ContactDto>> GetContactsAsync(string userId, int applicationId)
    {
        return await db.Contacts
            .Where(c => c.ApplicationId == applicationId && c.Application!.UserId == userId)
            .Select(c => new ContactDto(c.Id, c.ApplicationId, c.Name, c.Role, c.Email, c.Phone, c.LinkedInUrl))
            .ToListAsync();
    }

    public async Task<bool> UpdateContactAsync(string userId, int contactId, CreateContactDto dto)
    {
        var contact = await db.Contacts
            .Include(c => c.Application)
            .FirstOrDefaultAsync(c => c.Id == contactId && c.Application!.UserId == userId);

        if (contact is null) return false;

        contact.Name = dto.Name;
        contact.Role = dto.Role;
        contact.Email = dto.Email;
        contact.Phone = dto.Phone;
        contact.LinkedInUrl = dto.LinkedInUrl;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteContactAsync(string userId, int contactId)
    {
        var contact = await db.Contacts
            .Include(c => c.Application)
            .FirstOrDefaultAsync(c => c.Id == contactId && c.Application!.UserId == userId);

        if (contact is null) return false;

        db.Contacts.Remove(contact);
        await db.SaveChangesAsync();
        return true;
    }
}
