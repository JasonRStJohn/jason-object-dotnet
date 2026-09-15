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
            isPersistent: false, lockoutOnFailure: true);
        return result.Succeeded
            ? new AuthResult(true)
            : new AuthResult(false, "Invalid email or password.");
    }

    public async Task SignOutAsync() =>
        await _signInManager.SignOutAsync();

    public async Task<AuthResult> ChangePasswordAsync(
        ClaimsPrincipal principal, string currentPassword, string newPassword)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user is null)
            return new AuthResult(false, "Not signed in.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return new AuthResult(false, string.Join(" ", result.Errors.Select(e => e.Description)));

        // The security stamp changes on a password change, which would otherwise
        // invalidate the current cookie and silently sign the user out mid-request.
        await _signInManager.RefreshSignInAsync(user);
        return new AuthResult(true);
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal) =>
        await _userManager.GetUserAsync(principal);
}
