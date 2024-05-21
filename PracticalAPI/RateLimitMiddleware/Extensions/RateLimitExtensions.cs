using Microsoft.Extensions.Options;
using PracticalAPI.LimiterMiddleware.Models;
using PracticalAPI.RateLimitMiddleware.CustomRateLimit;
using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace PracticalAPI.RateLimitMiddleware.Extensions
{
    public static class RateLimitExtensions
    {
        public static IServiceCollection AddAppRateLimit(this IServiceCollection services
            , Action<RateLimitOptions> configureOptions)
        {
            services.Configure<RateLimitOptions>(configureOptions);
            var rateLimitConfigs = services.BuildServiceProvider().GetService<IOptionsSnapshot<RateLimitOptions>>()?.Value;

            return services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.OnRejected = (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
                        .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                        .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));

                    return new ValueTask();
                };

                limiterOptions.AddPolicy<string, CustomRateLimitPolicy>("CustomPolicy");
                limiterOptions.AddPolicy("User", context =>
                {
                    var username = "anonymous user";
                    if (context.User.Identity?.IsAuthenticated is true)
                    {
                        username = context.User.ToString()!;
                    }

                    return RateLimitPartition.GetSlidingWindowLimiter(username,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitConfigs?.PermitLimit ?? 5,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = rateLimitConfigs?.QueueLimit ?? 1,
                            Window = TimeSpan.FromSeconds(rateLimitConfigs?.Window ?? 3),
                            SegmentsPerWindow = rateLimitConfigs?.SegmentsPerWindow ?? 1,
                        });

                });

                limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                {
                    IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (remoteIpAddress != null && !IPAddress.IsLoopback(remoteIpAddress))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(remoteIpAddress,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = rateLimitConfigs?.PermitLimit ?? 5,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = rateLimitConfigs?.QueueLimit ?? 1,
                                Window = TimeSpan.FromSeconds(rateLimitConfigs?.Window ?? 3),
                            });
                    }

                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                });
            });
        }

        static string GetUserEndPoint(HttpContext context)
            => $"Hello {context.User.Identity?.Name ?? "Anonymous"} " +  $"Endpoint:{context.Request.Path} Method: {context.Request.Method}";
    }
}
