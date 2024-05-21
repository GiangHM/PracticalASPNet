using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using PracticalAPI.LimiterMiddleware.Models;
using System.Threading.RateLimiting;

namespace PracticalAPI.RateLimitMiddleware.CustomRateLimit
{
    public class CustomRateLimitPolicy : IRateLimiterPolicy<string>
    {
        private readonly Func<OnRejectedContext, CancellationToken, ValueTask>? _onRejected;
        private readonly RateLimitOptions _options;

        public CustomRateLimitPolicy(ILogger<CustomRateLimitPolicy> logger,
                                       IOptions<RateLimitOptions> options)
        {
            _onRejected = (ctx, token) =>
            {
                ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                logger.LogWarning($"Request rejected by {nameof(CustomRateLimitPolicy)}");
                return ValueTask.CompletedTask;
            };
            _options = options.Value;
        }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                return RateLimitPartition.GetFixedWindowLimiter(httpContext.User.Identity.Name!,
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = _options.PermitLimit,
                        Window = TimeSpan.FromMinutes(1),
                    });
            }

            return RateLimitPartition.GetFixedWindowLimiter(httpContext.Request.Headers.Host.ToString(),
                partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = _options.PermitLimit,
                    QueueLimit = _options.QueueLimit,
                    Window = TimeSpan.FromMinutes(1),
                });
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => _onRejected;
    }
}
