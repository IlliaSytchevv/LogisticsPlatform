using LogisticsPlatform.Application.Interfaces.Wrappers;
using LogisticsPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LogisticsPlatform.Infrastructure.Wrappers;

public class UserManagerWrapper(UserManager<ApplicationUser> userManager) : IUserManagerWrapper
{
    public Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return userManager.CreateAsync(user, password);
    }

    public Task<ApplicationUser?> FindByNameAsync(string username)
    {
        return userManager.FindByNameAsync(username);
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return userManager.CheckPasswordAsync(user, password);
    }

    public Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return userManager.GetRolesAsync(user);
    }

    public async Task AddToRoleAsync(ApplicationUser user, string role)
    {
        await userManager.AddToRoleAsync(user, role);
    }
}