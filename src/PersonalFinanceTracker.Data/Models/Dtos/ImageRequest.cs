namespace PersonalFinanceTracker.Data.Models.Dtos;

public class ImageRequest
{
    public string UserId { get; set; }
    public string? ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? FileExtension { get; set; }
    public double Size { get; set; }
    public string? ReadableSize { get; set; }
}

public class CreateImageRequestDto : ImageRequest
{
    public string CreatedBy { get; set; }
}

public class UpdateImageRequestDto : ImageRequest
{
    public string UpdatedBy { get; set; }
}