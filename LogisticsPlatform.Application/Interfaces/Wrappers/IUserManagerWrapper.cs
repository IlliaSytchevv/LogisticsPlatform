using LogisticsPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LogisticsPlatform.Application.Interfaces.Wrappers;

public interface IUserManagerWrapper
{
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

    Task<ApplicationUser?> FindByNameAsync(string username);

    Task<ApplicationUser?> FindByIdAsync(string userId);

    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

    Task<IList<string>> GetRolesAsync(ApplicationUser user);

    Task AddToRoleAsync(ApplicationUser user, string role);
}