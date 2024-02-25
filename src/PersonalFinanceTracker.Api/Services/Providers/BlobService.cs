using System.Security.Claims;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Options;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class BlobService(
    ILogger<BlobService> logger,
    BlobServiceClient blobServiceClient,
    IImageRepository imageRepo,
    IHttpContextAccessor httpContextAccessor,
    IOptionsMonitor<AzureBlobStorageConfig> azureBlobStorageConfigOpt,
    ILinkService linkService)
    : IBlobService
{
    private readonly string _containerName = azureBlobStorageConfigOpt.CurrentValue.ImageContainerName;

    private readonly string _userEmail =
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    private readonly string _userId =
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public async Task<IGenericApiResponse<List<BlobResponseDto>>> GetBlobsAsync()
    {
        try
        {
            logger.LogInformation("Getting blobs from container: {ContainerName}", _containerName);

            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var blobs = new List<BlobResponseDto>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var blobProperties = await blobClient.GetPropertiesAsync();
                var originalFileName = blobProperties.Value.Metadata.TryGetValue("OriginalFileName",
                    out var value)
                    ? value
                    : string.Empty;

                blobs.Add(new BlobResponseDto
                {
                    FileName = blobItem.Name,
                    OriginalFileName = originalFileName,
                    Url = blobClient.Uri.AbsoluteUri,
                    ReadableSize = blobItem.Properties.ContentLength?.ToReadableSize() ?? string.Empty,
                    CreatedAt = blobItem.Properties?.CreatedOn?.DateTime
                });
            }

            blobs.ForEach(AddLinksForBlob);

            return blobs.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<List<BlobResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<BlobResponseDto>> GetBlobsAsync(string blobName)
    {
        try
        {
            logger.LogInformation("Getting blob: {BlobName} from container: {ContainerName}", blobName,
                _containerName);

            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var blobProperties = await blobClient.GetPropertiesAsync();

            var originalFileName = blobProperties.Value.Metadata.TryGetValue("OriginalFileName",
                out var value)
                ? value
                : string.Empty;
            var response = new BlobResponseDto
            {
                FileName = blobName,
                OriginalFileName = originalFileName,
                Url = blobClient.Uri.AbsoluteUri,
                ReadableSize = blobProperties.Value.ContentLength.ToReadableSize(),
                CreatedAt = blobProperties.Value.CreatedOn.DateTime
            };

            await Task.CompletedTask;

            AddLinksForBlob(response);

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<BlobResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<BlobResponseDto>> UploadFileBlobAsync(IFormFile file)
    {
        try
        {
            logger.LogInformation("Uploading file: {FileName} to container: {ContainerName}", file.FileName,
                _containerName);

            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            var headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            var metadata = new Dictionary<string, string>
            {
                { "OriginalFileName", file.FileName }
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(), headers, metadata);

            if (result?.GetRawResponse()?.Status == StatusCodes.Status201Created)
            {
                var image = new Image
                {
                    UserId = _userId,
                    ImageUrl = blobClient.Uri.AbsoluteUri,
                    FilePath = blobClient.Name,
                    FileName = fileName,
                    OriginalFileName = file.FileName,
                    FileExtension = Path.GetExtension(file.FileName),
                    Size = file.Length,
                    ReadableSize = file.Length.ToReadableSize(),
                    CreatedBy = _userEmail
                };

                var added = await imageRepo.AddAsync(image);
                if (!added)
                    logger.LogError("Failed to save image to database. Image: {Image}",
                        JsonSerializer.Serialize(image));

                BlobResponseDto response = new()
                {
                    FileName = fileName,
                    OriginalFileName = file.FileName,
                    Url = blobClient.Uri.AbsoluteUri,
                    ReadableSize = file.Length.ToReadableSize(),
                    CreatedAt = DateTime.UtcNow
                };

                AddLinksForBlob(response);

                return response.ToOkApiResponse();
            }

            logger.LogError("Failed to upload file to blob storage. Result: {Result}",
                JsonSerializer.Serialize(result));

            return GenericApiResponse<BlobResponseDto>.Default.ToFailedDependenciesApiResponse(
                "Failed to upload file to blob storage. Please try again.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<BlobResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<string>> DeleteBlobAsync(string blobName)
    {
        try
        {
            logger.LogInformation("Deleting blob: {BlobName} from container: {ContainerName}", blobName,
                _containerName);

            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteIfExistsAsync();

            if (result)
            {
                var image = await imageRepo.GetAsQueryable().FirstOrDefaultAsync(x => x.FileName == blobName);
                if (image == null) return blobName.ToAcceptedApiResponse();
                var removed = await imageRepo.DeleteAsync(image);
                if (!removed)
                    logger.LogError("Failed to remove image from database. Image: {Image}",
                        JsonSerializer.Serialize(image));

                return blobName.ToAcceptedApiResponse();
            }

            logger.LogError("Failed to delete blob from blob storage. Result: {Result}", result);

            return GenericApiResponse<string>.Default.ToFailedDependenciesApiResponse(
                "Failed to delete blob from blob storage. Please try again.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return GenericApiResponse<string>.Default.ToInternalServerErrorApiResponse();
        }
    }

    private void AddLinksForBlob(BlobResponseDto response)
    {
        response?.Links.Add(
            linkService.GenerateLink("GetBlob", new { blobName = response.FileName }, "self", "GET"));
        response?.Links.Add(
            linkService.GenerateLink("DeleteBlob", new { blobName = response.FileName }, "delete-blob", "DELETE"));
    }
}