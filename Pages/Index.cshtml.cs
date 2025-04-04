using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace SecureWebApplication.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // Sanitize inputs
        Username = InputSanitizer.SanitizeInput(Username);
        Email = InputSanitizer.SanitizeInput(Email);

        // Validate email format
        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ModelState.AddModelError("Email", "Invalid email format.");
            return Page();
        }

        // Redirect to the Landing page after successful validation
        return RedirectToPage("/Landing", new { username = Username, email = Email });
    }
}
