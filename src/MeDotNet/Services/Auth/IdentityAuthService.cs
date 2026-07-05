using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MeDotNet.Models;

namespace MeDotNet.Services.Auth;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityAuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<AuthResult> SignInAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password,
            isPersistent: false, lockoutOnFailure: false);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, "Invalid email or password.");
    }

    public async Task SignOutAsync() =>
        await _signInManager.SignOutAsync();

    public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal) =>
        await _userManager.GetUserAsync(principal);
}
