using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("register", async ([FromServices] IAuthManager authManager, [FromBody] AppUserDto user) =>
            {
                var res = await authManager.RegisterUserAsync(user);
                return TypedResults.Json(res);
            })
            .AddEndpointFilter<ValidationFilter<AppUserDto>>()
            .WithName("RegisterUser")
            .Produces<LoginOrRegisterResponseDto>()
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Register a new user")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();

        group.MapPost("login", async ([FromServices] IAuthManager authManager, [FromBody] LoginUserDto user) =>
            {
                var res = await authManager.LoginAsync(user);
                return TypedResults.Json(res);
            })
            .AddEndpointFilter<ValidationFilter<LoginUserDto>>()
            .WithName("LoginUser")
            .Produces<LoginOrRegisterResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Login a user")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();

        group.MapPatch("refresh-token", async ([FromServices] IAuthManager authManager, [FromBody] LoginOrRegisterResponseDto user) =>
            {
                var res = await authManager.VerifyRefreshTokenAsync(user);
                return TypedResults.Json(res);
            })
            .WithName("RefreshToken")
            .Produces<RefreshTokenResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Refresh token")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();
    }
}