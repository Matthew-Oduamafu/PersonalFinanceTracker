using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IBlobService
{
    Task<IGenericApiResponse<List<BlobResponseDto>>> GetBlobsAsync();
    Task<IGenericApiResponse<BlobResponseDto>> GetBlobsAsync(string blobName);
    Task<IGenericApiResponse<BlobResponseDto>> UploadFileBlobAsync(IFormFile file);
    Task<IGenericApiResponse<string>> DeleteBlobAsync(string blobName);
}