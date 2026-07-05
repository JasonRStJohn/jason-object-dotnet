using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MeDotNet.Services.Auth;

namespace MeDotNet.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        public string Password { get; set; } = "";

        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // TODO Phase 2: restrict registration to authorized users or invites; currently any registered user can access /admin
        if (!ModelState.IsValid)
            return Page();

        var result = await _authService.RegisterAsync(Input.Email, Input.Password);
        if (result.Success)
            return LocalRedirect("/account/login");

        ModelState.AddModelError(string.Empty, result.ErrorMessage!);
        return Page();
    }
}
