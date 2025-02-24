using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ConstructionProjectManagement.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next; 
        } 

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request details here
            await _next(context);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}