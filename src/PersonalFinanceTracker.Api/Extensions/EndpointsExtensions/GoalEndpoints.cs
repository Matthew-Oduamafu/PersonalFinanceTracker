using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/goal")
            .RequireAuthorization()
            .WithTags("Goal");

        group.MapPost("",
                async (HttpContext context, [FromServices] IGoalService goalService,
                    [FromBody] CreateGoalRequestDto request) =>
                {
                    var user = UserData.GetUserEmail(context);
                    request.UserId = user.userId;
                    request.CreatedBy = user.email;

                    var response = await goalService.CreateGoalAsync(request);
                    return response.ToActionResult();
                })
            .AddEndpointFilter<ValidationFilter<CreateGoalRequestDto>>()
            .WithName("CreateGoal")
            .Produces<IGenericApiResponse<GoalResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new goal")
            .WithDescription(
                "Please provide a valid request.<br/>Kindly check payload `schema` for payload requirements.")
            .WithOpenApi();

        group.MapPut("{id}",
                async (HttpContext context, [FromServices] IGoalService goalService,
                    [FromBody] UpdateGoalRequestDto request, [FromRoute] string id) =>
                {
                    var user = UserData.GetUserEmail(context);
                    request.UserId = user.userId;
                    request.UpdatedBy = user.email;

                    var response = await goalService.UpdateGoalAsync(id, request);
                    return response.ToActionResult();
                }).AddEndpointFilter<ValidationFilter<UpdateGoalRequestDto>>()
            .WithName("UpdateGoal")
            .Produces<IGenericApiResponse<TransactionResponseDto>>(StatusCodes.Status202Accepted)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Update a goal")
            .WithDescription(
                "Please provide a valid request.<br/>Kindly check payload `schema` for payload requirements.")
            .WithOpenApi();

        group.MapGet("{id}",
                async ([FromServices] IGoalService goalService, [FromRoute] string id) =>
                {
                    var response = await goalService.GetGoalByIdAsync(id);
                    return response.ToActionResult();
                })
            .WithName("GetGoal")
            .Produces<IGenericApiResponse<TransactionResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a goal")
            .WithDescription(
                "Please provide a valid guid for an existing goal.")
            .WithOpenApi();

        group.MapGet("", [Authorize(Roles = "Super Administrator,Administrator")] async (
                [FromServices] IGoalService goalService,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                [FromQuery] string? userId,
                [FromQuery] string? name,
                [FromQuery] DateTime? targetDate,
                [FromQuery] DateTime? fromTargetDate,
                [FromQuery] DateTime? toTargetDate,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] string? sortDir) =>
            {
                var filter = new GoalFilter
                {
                    Page = page ?? 1,
                    PageSize = pageSize ?? 10,
                    UserId = userId,
                    Name = name,
                    TargetDate = targetDate,
                    FromTargetDate = fromTargetDate,
                    ToTargetDate = toTargetDate,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortDir = sortDir ?? "asc"
                };
                var response = await goalService.GetGoalsAsync(filter);
                return response.ToActionResult();
            })
            .WithName("GetAllGoals")
            .Produces<IGenericApiResponse<PagedList<TransactionResponseDto>>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all goals")
            .WithDescription(
                "Get all goals with the specified filter. <br/>If no filter is specified, it will return all goals.<br/>This operation can only be performed by Super Admin")
            .WithOpenApi();

        group.MapDelete("{id}", async ([FromServices] IGoalService goalService, [FromRoute] string id) =>
            {
                var response = await goalService.DeleteGoalAsync(id);
                return response.ToActionResult();
            })
            .WithName("DeleteGoal")
            .Produces<IGenericApiResponse<AccountResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete a goal")
            .WithDescription("Please provide a valid Id for an existing goal.")
            .WithOpenApi();
    }
}