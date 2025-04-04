using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SecureWebApplication.Pages;

public class IndexModel : PageModel
{
    private readonly AuthService _authService;

    public IndexModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // Sanitize inputs
        Username = InputSanitizer.SanitizeInput(Username);
        Password = InputSanitizer.SanitizeInput(Password);

        if (_authService.AuthenticateUser(Username, Password))
        {
            // Retrieve user roles from the database
            var roles = _authService.GetUserRoles(Username);

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user
            Console.WriteLine("Signing in user...");
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();
            Console.WriteLine("User signed in successfully.");

            return RedirectToPage("/Landing");
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Index");
    }
}
