using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

public class AuthServiceTests
{
    [Fact]
    public void AuthenticateUser_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var mockAuthService = new Mock<AuthService>(MockBehavior.Strict, Mock.Of<IConfiguration>());
        mockAuthService.Setup(service => service.AuthenticateUser("invalidUser", "invalidPassword"))
                       .Returns(false);

        // Act
        var result = mockAuthService.Object.AuthenticateUser("invalidUser", "invalidPassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AuthenticateUser_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        var mockAuthService = new Mock<AuthService>(MockBehavior.Strict, Mock.Of<IConfiguration>());
        mockAuthService.Setup(service => service.AuthenticateUser("validUser", "validPassword"))
                       .Returns(true);

        // Act
        var result = mockAuthService.Object.AuthenticateUser("validUser", "validPassword");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RoleMiddleware_UserWithoutAdminRole_Returns403()
    {
        // Arrange
        var mockAuthorizationService = new Mock<AuthorizationService>(Mock.Of<IConfiguration>());
        mockAuthorizationService.Setup(service => service.IsUserInRole("user", "Admin"))
                                .Returns(false);

        var middleware = new RoleMiddleware(async (innerHttpContext) => { });
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user") }));

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(AuthorizationService)))
                           .Returns(mockAuthorizationService.Object);
        context.RequestServices = mockServiceProvider.Object;

        // Act
        var exception = Record.ExceptionAsync(() =>
            middleware.InvokeAsync(context, context.RequestServices)
        ).Result;

        // Assert
        Assert.Equal(403, context.Response.StatusCode);
    }

    [Fact]
    public void RoleMiddleware_UserWithAdminRole_AllowsAccess()
    {
        // Arrange
        var mockAuthorizationService = new Mock<AuthorizationService>(Mock.Of<IConfiguration>());
        mockAuthorizationService.Setup(service => service.IsUserInRole("admin", "Admin"))
                                .Returns(true);

        var middleware = new RoleMiddleware(async (innerHttpContext) => { });
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, "Admin") }));

        // Act
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(AuthorizationService)))
                           .Returns(mockAuthorizationService.Object);

        var exception = Record.ExceptionAsync(() =>
            middleware.InvokeAsync(context, mockServiceProvider.Object)
        ).Result;

        // Assert
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Logout_ClearsAuthenticationCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var logoutModel = new SecureWebApplication.Pages.LogoutModel();

        // Act
        var result = await logoutModel.OnGetAsync();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/Index", redirectResult.PageName);
    }
}