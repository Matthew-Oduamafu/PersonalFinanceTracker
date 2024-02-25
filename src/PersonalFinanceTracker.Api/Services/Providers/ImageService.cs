using System.Text.Json;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class ImageService(
    ILogger<ImageService> logger,
    IImageRepository imageRepo,
    ILinkService linkService,
    IBlobService blobService)
    : IImageService
{
    public async Task<IGenericApiResponse<ImageResponseDto>> GetImageByIdAsync(string id)
    {
        try
        {
            logger.LogInformation("Getting image by id: {Id}", id);
            var image = await imageRepo.GetAsync(id);
            if (image == null)
            {
                logger.LogWarning("Image not found: {Id}", id);
                return GenericApiResponse<ImageResponseDto>.Default.ToNotFoundApiResponse();
            }

            var response = image.ToResponse();

            AddLinksForImage(response);

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<ImageResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<PagedList<ImageResponseDto>>> GetImagesAsync(ImageFilter filter)
    {
        try
        {
            logger.LogInformation("Getting images with filter: {@Filter}", JsonSerializer.Serialize(filter));

            var query = imageRepo.GetAsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FileExtension))
                query = query.Where(x => x.FileExtension == filter.FileExtension);

            if (!string.IsNullOrWhiteSpace(filter.CreatedBy)) query = query.Where(x => x.CreatedBy == filter.CreatedBy);

            if (!string.IsNullOrWhiteSpace(filter.OriginalFileName))
                query = query.Where(x =>
                    !string.IsNullOrEmpty(x.OriginalFileName) && x.OriginalFileName.Contains(filter.OriginalFileName,
                        StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.ReadableSize))
                query = query.Where(x =>
                    !string.IsNullOrEmpty(x.ReadableSize) && x.ReadableSize.Contains(filter.ReadableSize,
                        StringComparison.CurrentCultureIgnoreCase));

            if (filter.FromDate.HasValue) query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue) query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);

            query = filter.SortDir == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            var responseQuery = query.Select(x => x.ToResponse());

            var pagedList = await responseQuery.ToPagedList(filter.Page, filter.PageSize);

            AddLinksForPagedImages(pagedList, filter, linkService);

            return pagedList.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<PagedList<ImageResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<ImageResponseDto>> DeleteImageAsync(string id)
    {
        try
        {
            logger.LogInformation("Deleting image by id: {Id}", id);
            var image = await imageRepo.GetAsync(id);
            if (image == null)
            {
                logger.LogWarning("Image not found: {Id}", id);
                return GenericApiResponse<ImageResponseDto>.Default.ToNotFoundApiResponse();
            }

            var deleted = await imageRepo.DeleteAsync(image);
            if (deleted)
            {
                logger.LogInformation("Image deleted from database: {Id}", id);

                var deletedFromBlob = await blobService.DeleteBlobAsync(image.FileName ?? string.Empty);
                if (deletedFromBlob.Code == StatusCodes.Status200OK)
                    logger.LogInformation("Image deleted from blob storage: {Id}", id);
                else
                    logger.LogWarning("Failed to delete image from blob storage: {Id}", id);

                return image.ToResponse().ToOkApiResponse();
            }

            logger.LogError("Failed to delete image from database. Image: {Image}", JsonSerializer.Serialize(image));
            return GenericApiResponse<ImageResponseDto>.Default.ToFailedDependenciesApiResponse(
                "Failed to delete image from database. Please try again.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<ImageResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    private void AddLinksForImage(ImageResponseDto response)
    {
        response?.Links.Add(
            linkService.GenerateLink("GetImage", new { id = response.Id }, "self", "GET"));
        response?.Links.Add(
            linkService.GenerateLink("DeleteImage", new { id = response.Id }, "delete-image", "DELETE"));
    }

    private static void AddLinksForPagedImages(PagedList<ImageResponseDto> apiResponse, BaseFilter filter,
        ILinkService linkService)
    {
        if (apiResponse?.Items == null || !apiResponse.Items.Any()) return;

        apiResponse.Links.Add(
            linkService.GenerateLink("GetAllImages",
                new { filter.Page, filter.PageSize }, "self", "GET"));

        if (apiResponse.Page > 1)
            apiResponse.Links.Add(
                linkService.GenerateLink("GetAllImages",
                    new { Page = filter.Page - 1, filter.PageSize }, "previous-page", "GET"));

        if (apiResponse.Page < apiResponse.TotalPages)
            apiResponse.Links.Add(
                linkService.GenerateLink("GetAllImages",
                    new { Page = filter.Page + 1, filter.PageSize }, "next-page", "GET"));
    }
}