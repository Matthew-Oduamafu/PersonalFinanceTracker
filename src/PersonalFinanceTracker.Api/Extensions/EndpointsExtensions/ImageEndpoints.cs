using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/images")
            .RequireAuthorization()
            .WithTags("Images");

        group.MapGet("", async ([FromServices] IImageService imageService,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                [FromQuery] string? originalFileName,
                [FromQuery] string? fileExtension,
                [FromQuery] string? readableSize,
                [FromQuery] string? createdBy,
                [FromQuery] DateTime? fromDate,
                [FromQuery] DateTime? toDate,
                [FromQuery] string? sortDir) =>
            {
                var filter = new ImageFilter
                {
                    Page = page ?? 1,
                    PageSize = pageSize ?? 10,
                    OriginalFileName = originalFileName,
                    FileExtension = fileExtension,
                    ReadableSize = readableSize,
                    CreatedBy = createdBy,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortDir = sortDir ?? "asc"
                };
                var response = await imageService.GetImagesAsync(filter);
                return response.ToActionResult();
            })
            .WithName("GetAllImages")
            .Produces<IGenericApiResponse<PagedList<ImageResponseDto>>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all images")
            .WithDescription(
                "Get all images with the specified filter. <br/>If no filter is specified, it will return all images.<br/>This operation can only be performed by Super Admin")
            .WithOpenApi();

        group.MapGet("/{id}",
                async ([FromServices] IImageService imageService, [FromRoute] string id) =>
                {
                    var response = await imageService.GetImageByIdAsync(id);
                    return response.ToActionResult();
                })
            .WithName("GetImage")
            .Produces<IGenericApiResponse<ImageResponseDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get an image")
            .WithDescription(
                "Get an image by its id.")
            .WithOpenApi();

        group.MapDelete("/{id}",
                async ([FromServices] IImageService imageService, [FromRoute] string id) =>
                {
                    var response = await imageService.DeleteImageAsync(id);
                    return response.ToActionResult();
                })
            .WithName("DeleteImage")
            .Produces<IGenericApiResponse<ImageResponseDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete an image")
            .WithDescription(
                "Delete an image by its id.")
            .WithOpenApi();
    }
}