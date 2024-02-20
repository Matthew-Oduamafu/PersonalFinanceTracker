using System.Text.Json;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;
    private readonly IImageRepository _imageRepo;
    private IBlobService _blobService;

    public ImageService(ILogger<ImageService> logger, IImageRepository imageRepo, IBlobService blobService)
    {
        _logger = logger;
        _imageRepo = imageRepo;
        _blobService = blobService;
    }

    public async Task<IGenericApiResponse<ImageResponseDto>> GetImageByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Getting image by id: {Id}", id);
            var image = await _imageRepo.GetAsync(id);
            if (image == null)
            {
                _logger.LogWarning("Image not found: {Id}", id);
                return GenericApiResponse<ImageResponseDto>.Default.ToNotFoundApiResponse();
            }
            
            return image.ToResponse().ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<ImageResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<PagedList<ImageResponseDto>>> GetImagesAsync(ImageFilter filter)
    {
        try
        {
            _logger.LogInformation("Getting images with filter: {@Filter}", JsonSerializer.Serialize(filter));
            
            var query = _imageRepo.GetAsQueryable();
            
            if (!string.IsNullOrWhiteSpace(filter.FileExtension))
            {
                query = query.Where(x => x.FileExtension == filter.FileExtension);
            }
            
            if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }
            
            if (!string.IsNullOrWhiteSpace(filter.OriginalFileName))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.OriginalFileName) && x.OriginalFileName.Contains(filter.OriginalFileName, StringComparison.CurrentCultureIgnoreCase));
            }
            
            if (!string.IsNullOrWhiteSpace(filter.ReadableSize))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.ReadableSize) && x.ReadableSize.Contains(filter.ReadableSize, StringComparison.CurrentCultureIgnoreCase));
            }
            
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
            }
            
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);
            }
            
            query = filter.SortDir == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
            
            var responseQuery = query.Select(x => x.ToResponse());
            
            var pagedList = await responseQuery.ToPagedList(filter.Page, filter.PageSize);
            
            return pagedList.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<PagedList<ImageResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<ImageResponseDto>> DeleteImageAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deleting image by id: {Id}", id);
            var image = await _imageRepo.GetAsync(id);
            if (image == null)
            {
                _logger.LogWarning("Image not found: {Id}", id);
                return GenericApiResponse<ImageResponseDto>.Default.ToNotFoundApiResponse();
            }
            
            var deleted = await _imageRepo.DeleteAsync(image);
            if (deleted)
            {
                _logger.LogInformation("Image deleted from database: {Id}", id);
                
                var deletedFromBlob = await _blobService.DeleteBlobAsync(image.FileName ?? string.Empty);
                if (deletedFromBlob.Code == StatusCodes.Status200OK)
                {
                    _logger.LogInformation("Image deleted from blob storage: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete image from blob storage: {Id}", id);
                }
                
                return image.ToResponse().ToOkApiResponse();
            }
            
            _logger.LogError("Failed to delete image from database. Image: {Image}", JsonSerializer.Serialize(image));
            return GenericApiResponse<ImageResponseDto>.Default.ToFailedDependenciesApiResponse("Failed to delete image from database. Please try again.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<ImageResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }
}