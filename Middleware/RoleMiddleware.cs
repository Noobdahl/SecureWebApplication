using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;

    public RoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        // Check if the user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            Console.WriteLine("User is not authenticated.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Please log in.");
            return;
        }

        if (context.User.Identity == null || string.IsNullOrEmpty(context.User.Identity.Name))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Please log in.");
            return;
        }

        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        // Resolve AuthorizationService and check roles
        var authorizationService = serviceProvider.GetRequiredService<AuthorizationService>();
        if (!authorizationService.IsUserInRole(context.User.Identity.Name, "Admin"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access Denied: You do not have permission to access this page.");
            return;
        }

        await _next(context);
    }
}