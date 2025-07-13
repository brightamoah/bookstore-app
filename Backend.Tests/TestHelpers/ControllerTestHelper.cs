using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Backend.Tests.TestHelpers;

public static class ControllerTestHelper
{
    public static T CreateController<T>(IServiceProvider? serviceProvider = null) where T : ControllerBase, new()
    {
        var controller = new T();
        var httpContext = CreateHttpContext(serviceProvider);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            ActionDescriptor = new ControllerActionDescriptor()
        };
        return controller;
    }

    public static T CreateControllerWithDependencies<T>(IServiceProvider serviceProvider) where T : ControllerBase
    {
        var controller = (T)ActivatorUtilities.CreateInstance(serviceProvider, typeof(T));
        var httpContext = CreateHttpContext(serviceProvider);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            ActionDescriptor = new ControllerActionDescriptor()
        };
        return controller;
    }

    public static HttpContext CreateHttpContext(IServiceProvider? serviceProvider = null)
    {
        var httpContext = new DefaultHttpContext();

        if (serviceProvider != null)
        {
            httpContext.RequestServices = serviceProvider;
        }

        // Add a mock trace identifier
        httpContext.TraceIdentifier = Guid.NewGuid().ToString();

        return httpContext;
    }

    public static void SetAuthenticatedUser(ControllerBase controller, int userId, string email = "test@example.com")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new("sub", userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext.HttpContext.User = principal;
    }

    public static void SetJwtCookie(ControllerBase controller, string jwtToken)
    {
        controller.Request.Headers.Cookie = $"jwtToken={jwtToken}";
        var cookies = new Mock<IRequestCookieCollection>();
        cookies.Setup(c => c["jwtToken"]).Returns(jwtToken);
        var request = new Mock<HttpRequest>();
        request.Setup(r => r.Cookies).Returns(cookies.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers.Cookie = $"jwtToken={jwtToken}";
    }

    public static IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            {"Jwt:Key", "ThisIsATestSecretKeyForJwtTokenGenerationThatIsLongEnough"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }

    public static IServiceProvider CreateTestServices()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton(CreateTestConfiguration());
        return services.BuildServiceProvider();
    }
}
