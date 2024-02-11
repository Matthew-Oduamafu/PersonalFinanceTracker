using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Data.Data.Entities;

public class Image : BaseEntity
{
    [MaxLength(200)]
    public string? ImageUrl { get; set; }
    [MaxLength(200)]
    public string? FilePath { get; set; }
    [MaxLength(200)]
    public string? FileName { get; set; }
    [MaxLength(10)]
    public string? FileExtension { get; set; }
    public double Size { get; set; }
    [MaxLength(50)]
    public string? ReadableSize { get; set; }
}