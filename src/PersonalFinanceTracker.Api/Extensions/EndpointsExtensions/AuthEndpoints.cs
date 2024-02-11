using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.CustomFilters;
using PersonalFinanceTracker.Api.Models;
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
                return res.ToActionResult();
            })
            .AddEndpointFilter<ValidationFilter<AppUserDto>>()
            .WithName("RegisterUser")
            .Produces<IGenericApiResponse<LoginOrRegisterResponseDto>>(StatusCodes.Status201Created)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Register a new user")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();

        group.MapPost("login", async ([FromServices] IAuthManager authManager, [FromBody] LoginUserDto user) =>
            {
                var res = await authManager.LoginAsync(user);
                return res.ToActionResult();
            })
            .AddEndpointFilter<ValidationFilter<LoginUserDto>>()
            .WithName("LoginUser")
            .Produces<IGenericApiResponse<LoginOrRegisterResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Login a user")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();

        group.MapPatch("refresh-token",
                async ([FromServices] IAuthManager authManager, [FromBody] LoginOrRegisterResponseDto user) =>
                {
                    var res = await authManager.VerifyRefreshTokenAsync(user);
                    return res.ToActionResult();
                })
            .WithName("RefreshToken")
            .Produces<IGenericApiResponse<LoginOrRegisterResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Refresh token")
            .WithDescription("Some description here<br/>Next line description here")
            .WithOpenApi();
    }
}