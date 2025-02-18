using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace E_CommerceApplication.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class StatsMiddleware
    {
        private readonly RequestDelegate _next;

        public StatsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // my Class Midlleware
        public Task Invoke(HttpContext httpContext)
        {
            DateTime requestTime = DateTime.Now;
            var result = _next(httpContext);
            DateTime responseTime = DateTime.Now;
            TimeSpan processDuration = responseTime - requestTime;
            Console.WriteLine($"[Stats middlware] process duration {processDuration.TotalMilliseconds} ms");
            return result;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StatsMiddleware>();
        }
    }
}
