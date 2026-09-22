namespace JobTracker.Api.DTOs;

public record ContactDto(int Id, int ApplicationId, string Name, string? Role, string? Email, string? Phone, string? LinkedInUrl);

public record CreateContactDto(string Name, string? Role, string? Email, string? Phone, string? LinkedInUrl);
