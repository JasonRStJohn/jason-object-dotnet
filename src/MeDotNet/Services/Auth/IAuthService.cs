using System.Security.Claims;
using MeDotNet.Models;

namespace MeDotNet.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password);
    Task<AuthResult> SignInAsync(string email, string password);
    Task SignOutAsync();

    /// <summary>
    /// Changes the signed-in user's password and refreshes their auth cookie so the
    /// session survives the change. Requires the current password: without it, anyone
    /// who found an unlocked browser could lock the owner out of their own site.
    /// </summary>
    Task<AuthResult> ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword);
    Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
}
