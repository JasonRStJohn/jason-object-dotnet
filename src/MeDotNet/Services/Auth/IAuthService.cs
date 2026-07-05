using System.Security.Claims;
using MeDotNet.Models;

namespace MeDotNet.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password);
    Task<AuthResult> SignInAsync(string email, string password);
    Task SignOutAsync();
    Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
}
