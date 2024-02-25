namespace PersonalFinanceTracker.Data.Models.Dtos;

#pragma warning disable CS8618
public class ImageResponseDto
{
    public string Id { get; set; }
    public string? UserId { get; set; }
    public string? ImageUrl { get; set; }
    public string? FilePath { get; set; }
    public string? FileName { get; set; }
    public string? OriginalFileName { get; set; }
    public string? FileExtension { get; set; }
    public double Size { get; set; }
    public string? ReadableSize { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Link> Links { get; set; }
}