namespace PersonalFinanceTracker.Data.Data.Entities;

#pragma warning disable CS8618

public class Goal : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
}