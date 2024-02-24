using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.Models;

namespace PersonalFinanceTracker.Api.CustomMiddleware;

/***
 * .NET 8 introduced a new way to handle exceptions globally using IExceptionHandler
 */
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Something went wrong: {ExMessage}", exception.Message);
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        // var problemDetails = new ProblemDetails
        // {
        //     Status = httpContext.Response.StatusCode,
        //     Title = "Oops! Internal Server Error.",
        //     Detail = exception.Message,
        //     Instance = httpContext.Request.Path,
        //     Extensions = new Dictionary<string, object?>()
        // };
        
        // var response = new ExceptionDetails
        // {
        //     StatusCode = httpContext.Response.StatusCode,
        //     Message = "Oops! Internal Server Error.",
        //     Details = exception.Message
        // };
        
        var response = GenericApiResponse<object>.Default.ToInternalServerErrorApiResponse();

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}