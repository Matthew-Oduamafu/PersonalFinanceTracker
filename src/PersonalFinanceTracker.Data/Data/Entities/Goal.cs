using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Goal : BaseEntity
{
    [MaxLength(36)] public string UserId { get; set; }

    [MaxLength(100)] public string Name { get; set; }

    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
}