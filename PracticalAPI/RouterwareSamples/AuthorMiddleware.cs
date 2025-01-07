using System.Diagnostics;
using System.Reflection;

namespace PracticalAPI.RouterwareSamples
{
    public class AuthorMiddleware
    {
        readonly RequestDelegate _next;

        public AuthorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsync("Victor");
        }
    }
}
