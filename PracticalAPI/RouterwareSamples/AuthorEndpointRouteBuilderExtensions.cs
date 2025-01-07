using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Session;

namespace PracticalAPI.RouterwareSamples
{
    public static class AuthorEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapAuthor(this IEndpointRouteBuilder endpoints, string pattern)
        {
            var pipeline = endpoints.CreateApplicationBuilder()
              .UseMiddleware<AuthorMiddleware>()
              .Build();

            return endpoints.Map(pattern, pipeline).WithDisplayName("DefaultDisplayName");
        }
    }
}
