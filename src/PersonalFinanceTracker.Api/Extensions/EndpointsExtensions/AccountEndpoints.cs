using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
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

        group.MapPost("",
                async (HttpContext context, [FromServices] IAccountService accountService,
                    [FromBody] CreateAccountRequestDto request) =>
                {
                    var user = UserData.GetUserEmail(context);
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

        group.MapPut("{id}",
                async (HttpContext context, [FromServices] IAccountService accountService,
                    [FromBody] UpdateAccountRequestDto request, [FromRoute] string id) =>
                {
                    var user = UserData.GetUserEmail(context);
                    request.UpdatedBy = user.email;
                    request.UserId = user.userId;

                    var response = await accountService.UpdateAccountAsync(id, request);
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

        group.MapGet("", [Authorize(Roles = "Super Administrator,Administrator")] async (
                [FromServices] IAccountService accountService,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                [FromQuery] string? name,
                [FromQuery] string? accountType,
                [FromQuery] string? createdBy,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] string? sortDir) =>
            {
                var filter = new AccountFilter
                {
                    Page = page ?? 1,
                    PageSize = pageSize ?? 10,
                    Name = name,
                    AccountType = accountType,
                    CreatedBy = createdBy,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortDir = sortDir ?? "asc"
                };
                var response = await accountService.GetAccountsAsync(filter);
                return response.ToActionResult();
            })
            .WithName("GetAll")
            .Produces<IGenericApiResponse<PagedList<AccountResponseDto>>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all accounts")
            .WithDescription(
                "Get all accounts with the specified filter. <br/>If no filter is specified, it will return all accounts.<br/>This operation can only be performed by Super Admin")
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
}