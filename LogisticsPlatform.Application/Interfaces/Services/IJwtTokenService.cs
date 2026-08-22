using LogisticsPlatform.Domain.Entities;

namespace LogisticsPlatform.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}