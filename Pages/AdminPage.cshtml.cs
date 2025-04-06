using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace SecureWebApplication.Pages;

[Authorize(Roles = "Admin")]
public class AdminPageModel : PageModel
{
    private readonly IConfiguration _configuration;

    public AdminPageModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return Page();
        }

        try
        {
            // Hash the password
            var hashedPassword = HashPassword(Password);

            // Insert the user into the database
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash, Email) VALUES (@Username, @PasswordHash, @Email)", connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    command.Parameters.AddWithValue("@Email", Email);

                    command.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return Page();
        }
    }

    private byte[] HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}