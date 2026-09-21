using JobTracker.Api.Models;

namespace JobTracker.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(ApplicationUser user);
}
