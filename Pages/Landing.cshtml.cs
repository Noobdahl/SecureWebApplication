using Microsoft.AspNetCore.Mvc.RazorPages;

public class LandingModel : PageModel
{
    public string Username { get; set; } = string.Empty;

    public void OnGet(string username)
    {
        Username = username;
    }
}