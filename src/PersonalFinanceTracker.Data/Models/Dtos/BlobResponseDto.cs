namespace PersonalFinanceTracker.Data.Models.Dtos;

public class BlobResponseDto
{
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string Url { get; set; }
    public string ReadableSize { get; set; }
    public DateTime? CreatedAt { get; set; }
}