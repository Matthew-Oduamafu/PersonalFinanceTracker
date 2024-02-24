using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomMiddleware;

namespace PersonalFinanceTracker.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        return app.UseMiddleware<GlobalExceptionHandler>(logger);
    }
}