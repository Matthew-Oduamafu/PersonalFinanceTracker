namespace PersonalFinanceTracker.Api.Models.RequestFilters;

public class ImageFilter : BaseFilter
{
    public string? OriginalFileName { get; set; }
    public string? FileExtension { get; set; }
    public string? ReadableSize { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}