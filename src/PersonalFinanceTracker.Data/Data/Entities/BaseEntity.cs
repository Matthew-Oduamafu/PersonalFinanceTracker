using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618
public abstract class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [MaxLength(36)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [MaxLength(100)] 
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}