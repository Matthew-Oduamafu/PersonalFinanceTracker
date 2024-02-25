namespace PersonalFinanceTracker.Data.Models.Dtos;

public class GoalResponseDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Link> Links { get; set; }
}