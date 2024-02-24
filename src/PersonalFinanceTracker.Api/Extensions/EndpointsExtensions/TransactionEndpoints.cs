using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/transaction")
            .RequireAuthorization()
            .WithTags("Transaction");

        group.MapPost("",
                async (HttpContext context, [FromServices] ITransactionService transactionService,
                    [FromBody] CreateTransactionRequestDto request) =>
                {
                    var user = UserData.GetUserEmail(context);
                    request.CreatedBy = user.email;

                    var response = await transactionService.CreateTransactionAsync(request);
                    return response.ToActionResult();
                })
            .AddEndpointFilter<ValidationFilter<CreateTransactionRequestDto>>()
            .WithName("CreateTransaction")
            .Produces<IGenericApiResponse<TransactionResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new transaction")
            .WithDescription(
                "Please provide a valid request.<br/>Kindly check payload `schema` for payload requirements.")
            .WithOpenApi();


        group.MapPut("{id}",
                [Authorize(Roles = "Super Administrator,Administrator")]
                async (HttpContext context, [FromServices] ITransactionService transactionService,
                    [FromBody] UpdateTransactionRequestDto request, [FromRoute] string id) =>
                {
                    var user = UserData.GetUserEmail(context);
                    request.UpdatedBy = user.email;

                    var response = await transactionService.UpdateTransactionAsync(id, request);
                    return response.ToActionResult();
                })
            .AddEndpointFilter<ValidationFilter<UpdateTransactionRequestDto>>()
            .WithName("UpdateTransaction")
            .Produces<IGenericApiResponse<TransactionResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Update a transaction")
            .WithDescription(
                "Please provide a valid request.<br/>Kindly check payload `schema` for payload requirements.")
            .WithOpenApi();

        group.MapGet("{id}", async ([FromServices] ITransactionService transactionService, [FromRoute] string id) =>
            {
                var response = await transactionService.GetTransactionByIdAsync(id);
                return response.ToActionResult();
            })
            .WithName("GetTransaction")
            .Produces<IGenericApiResponse<TransactionResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a transaction")
            .WithDescription(
                "Please provide a valid guid for an existing transaction.")
            .WithOpenApi();

        group.MapGet("", [Authorize(Roles = "Super Administrator,Administrator")] async ([FromServices] ITransactionService transactionService,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                [FromQuery] string? accountId,
                [FromQuery] string? description,
                [FromQuery] decimal? amount,
                [FromQuery] string? transactionType,
                [FromQuery] DateTime? fromTransactionDate,
                [FromQuery] DateTime? toTransactionDate,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] string? sortDir) =>
            {
                var filter = new TransactionFilter
                {
                    Page = page ?? 1,
                    PageSize = pageSize ?? 10,
                    AccountId = accountId,
                    Description = description,
                    Amount = amount,
                    TransactionType = transactionType,
                    FromTransactionDate = fromTransactionDate,
                    ToTransactionDate = toTransactionDate,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortDir = sortDir ?? "asc"
                };
                var response = await transactionService.GetTransactionsAsync(filter);
                return response.ToActionResult();
            })
            .WithName("GetAllTransactions")
            .Produces<IGenericApiResponse<PagedList<TransactionResponseDto>>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all transactions")
            .WithDescription(
                "Get all transactions with the specified filter. <br/>If no filter is specified, it will return all transactions.<br/>This operation can only be performed by Super Admin")
            .WithOpenApi();
    }
}