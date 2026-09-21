namespace JobTracker.Api.DTOs;

public record RegisterDto(string Email, string Password, string? DisplayName);

public record LoginDto(string Email, string Password);

public record AuthResponseDto(string Token, DateTime ExpiresAt, string Email, string? DisplayName);
