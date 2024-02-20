using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.DtoExtensions;

public static class ImageDtoExtensions
{
    public static ImageResponseDto ToResponse(this Image obj)
    {
        return new ImageResponseDto
        {
            Id = obj.Id,
            UserId = obj.UserId,
            ImageUrl = obj.ImageUrl,
            FilePath = obj.FilePath,
            FileName = obj.FileName,
            OriginalFileName = obj.OriginalFileName,
            FileExtension = obj.FileExtension,
            Size = obj.Size,
            ReadableSize = obj.ReadableSize,
            CreatedBy = obj.CreatedBy,
            CreatedAt = obj.CreatedAt
        };
    }
}