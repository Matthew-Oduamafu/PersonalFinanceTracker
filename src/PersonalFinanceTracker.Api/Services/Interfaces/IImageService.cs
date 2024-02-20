using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IImageService
{
    Task<IGenericApiResponse<ImageResponseDto>> GetImageByIdAsync(string id);
    Task<IGenericApiResponse<PagedList<ImageResponseDto>>> GetImagesAsync(ImageFilter filter);
    Task<IGenericApiResponse<ImageResponseDto>> DeleteImageAsync(string id);
}