using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;

public static class BlobEndpoints
{
    public static void MapBlobEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/blobs")
            .RequireAuthorization()
            .WithTags("Blobs");

        group.MapGet("get-anti-forgery-token", async (IAntiforgery antiforgery, HttpContext httpContext) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(httpContext);
                
                await Task.CompletedTask;
                
                return (new { tokens.RequestToken, tokens.HeaderName }).ToOkApiResponse().ToActionResult();
            })
            .AllowAnonymous()
            .WithName("GetAntiForgeryTokens")
            .Produces<IGenericApiResponse<List<string>>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Anti Forgery Tokens")
            .WithDescription(
                "Get the anti-forgery tokens to be used in the request headers for the POST, PUT, and DELETE operations")
            .WithOpenApi();

        group.MapGet("", async ([FromServices] IBlobService blobService) =>
            {
                var response = await blobService.GetBlobsAsync();
                return response.ToActionResult();
            })
            .WithName("GetAllBlobs")
            .Produces<IGenericApiResponse<List<BlobResponseDto>>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all blobs")
            .WithDescription(
                "Get all blobs in the specified container. <br/>If no container is specified, it will return all blobs in the storage account.<br/>This operation can only be performed by Super Admin")
            .WithOpenApi();

        group.MapGet("/{blobName}",
                async ([FromServices] IBlobService blobService, [FromRoute] string blobName) =>
                {
                    var response = await blobService.GetBlobsAsync(blobName);
                    return response.ToActionResult();
                })
            .WithName("GetBlob")
            .Produces<IGenericApiResponse<BlobResponseDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a blob")
            .WithDescription(
                "Get a blob from the specified container by its name. <br/>This operation can only be performed by Super Admin")
            .WithOpenApi();

        group.MapPost("",
                async ([FromServices] IBlobService blobService,
                    [FromForm] IFormFile file) =>
                {
                    var response = await blobService.UploadFileBlobAsync(file);
                    return response.ToActionResult();
                })
            .WithName("UploadBlob")
            .Produces<IGenericApiResponse<BlobResponseDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Upload a blob")
            .WithDescription(
                "Upload a blob to the specified container by providing the file. <br/>This operation can only be performed by Super Admin")
            .WithOpenApi();

        group.MapDelete("/{blobName}",
                async ([FromServices] IBlobService blobService, [FromRoute] string blobName) =>
                {
                    var response = await blobService.DeleteBlobAsync(blobName);
                    return response.ToActionResult();
                })
            .WithName("DeleteBlob")
            .Produces<IGenericApiResponse<string>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status424FailedDependency)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete a blob")
            .WithDescription(
                "Remove a blob from the specified container by its name. <br/>This operation can only be performed by Super Admin")
            .WithOpenApi();
    }
}