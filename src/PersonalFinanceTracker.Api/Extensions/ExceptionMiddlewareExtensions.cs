using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomMiddleware;

namespace PersonalFinanceTracker.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        return app.UseMiddleware<ExceptionMiddleware>(logger);
    }

    public static IServiceCollection AddModelValidation(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "See the errors property for details.",
                    Instance = context.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails);
            };
        });

        return services;
    }
}