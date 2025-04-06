using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LandingModel : PageModel
{
    private readonly CacheService _cacheService;

    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public LandingModel(CacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public void OnGet()
    {
        Username = HttpContext.User?.Identity?.Name ?? string.Empty;
    }

    public IActionResult OnPostLoadProducts()
    {
        if (_cacheService.TryGetProducts(out List<Product> products))
        {
            Message = "Products loaded from cache: " + string.Join(", ", products.Select(p => p.Name));
        }
        else
        {
            Message = "Products loaded from dB: " + string.Join(", ", products.Select(p => p.Name));
        }
        Console.WriteLine(Message);
        return Page();
    }
}