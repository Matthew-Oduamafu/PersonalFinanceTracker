using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/account")
            .RequireAuthorization()
            .WithTags("Account");

        group.MapPost("", async (HttpContext context, [FromServices] IAccountService accountService, [FromBody] CreateAccountRequestDto request) =>
            {
                var user = GetUserEmail(context);
                request.CreatedBy = user.email;
                request.UserId = user.userId;
                
                var response = await accountService.CreateAccountAsync(request);
                return response.ToActionResult();
                
            })
            .AddEndpointFilter<ValidationFilter<CreateAccountRequestDto>>()
            .WithName("Create")
            .Produces<IGenericApiResponse<AccountResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new account")
            .WithDescription("Please provide a valid request.<br/>Kindly payload `schema` for payload requirements.")
            .WithOpenApi();

        group.MapPut("{id}", async (HttpContext context, [FromServices] IAccountService accountService, [FromBody] UpdateAccountRequestDto request, [FromRoute] string id) =>
            {
                var user = GetUserEmail(context);
                request.UpdatedBy = user.email;
                request.UserId = user.userId;
                
                var response = await accountService.UpdateAccountAsync(request);
                return response.ToActionResult();
                
            })
            .AddEndpointFilter<ValidationFilter<UpdateAccountRequestDto>>()
            .WithName("Update")
            .Produces<IGenericApiResponse<AccountResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an account")
            .WithDescription("Please provide a valid request.<br/>Kindly payload `schema` for payload requirements.")
            .WithOpenApi();

        group.MapGet("{id}", async ([FromServices] IAccountService accountService, [FromRoute] string id) =>
            {
                var response = await accountService.GetAccountAsync(id);
                return response.ToActionResult();
                
            })
            .WithName("Get")
            .Produces<IGenericApiResponse<AccountResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get an account")
            .WithDescription("Please provide a valid Id for an existing account.")
            .WithOpenApi();

        group.MapDelete("{id}", async ([FromServices] IAccountService accountService, [FromRoute] string id) =>
            {
                var response = await accountService.DeleteAccountAsync(id);
                return response.ToActionResult();
                
            })
            .WithName("Delete")
            .Produces<IGenericApiResponse<AccountResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete an account")
            .WithDescription("Please provide a valid Id for an existing account.")
            .WithOpenApi();
        
    }

    private static (string email, string userId) GetUserEmail(HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(claim => claim.Type.EndsWith("emailaddress"))?.Value ??
                        string.Empty;
        var userId = context.User.Claims.FirstOrDefault(claim => claim.Type.EndsWith("nameidentifier"))?.Value ??
                        string.Empty;
        return (userEmail, userId);
    }
}