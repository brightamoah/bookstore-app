using System.Text.Json;
using AspNetCoreRateLimit;
using Backend.Models;

namespace Backend.Helpers;

public class RateLimitConfig
{
    public static void ConfigureRateLimit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.ClientIdHeader = "X-ClientId";

            options.QuotaExceededResponse = new QuotaExceededResponse
            {
                Content = "Rate limit exceeded. Please try again later.",
                ContentType = "application/json",
                StatusCode = 429
            };

            options.GeneralRules =
            [
                // Authentication endpoints (more restrictive)
                new RateLimitRule
                {
                    Endpoint = "POST:/api/login",
                    Period = "1m",
                    Limit = 5
                },
                new RateLimitRule
                {
                    Endpoint = "POST:/api/signup",
                    Period = "1m",
                    Limit = 3
                },
                new RateLimitRule
                {
                    Endpoint = "POST:/api/login",
                    Period = "1h",
                    Limit = 20
                },
                new RateLimitRule
                {
                    Endpoint = "POST:/api/signup",
                    Period = "1h",
                    Limit = 10
                },
                // Book management endpoints
                new RateLimitRule
                {
                    Endpoint = "POST:/api/books",
                    Period = "1m",
                    Limit = 10
                },
                new RateLimitRule
                {
                    Endpoint = "PUT:/api/books/*",
                    Period = "1m",
                    Limit = 15
                },
                new RateLimitRule
                {
                    Endpoint = "DELETE:/api/books/*",
                    Period = "1m",
                    Limit = 5
                },
                new RateLimitRule
                {
                    Endpoint = "GET:/api/books",
                    Period = "1m",
                    Limit = 50
                },
                new RateLimitRule
                {
                    Endpoint = "GET:/api/books/*",
                    Period = "1m",
                    Limit = 30
                },
                new RateLimitRule
                {
                    Endpoint = "GET:/api/user",
                    Period = "1m",
                    Limit = 20
                },


                new() {
                    Endpoint = "*",
                    Period = "1m",
                    Limit = 100
                },
                new() {
                    Endpoint = "*",
                    Period = "1h",
                    Limit = 1000
                }
            ];

            options.EndpointWhitelist =
            [
                "GET:/health",
                "GET:/swagger/*",
                "GET:/uploads/*"
            ];
        });

        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }
}