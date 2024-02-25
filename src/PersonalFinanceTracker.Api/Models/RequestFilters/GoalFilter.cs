namespace PersonalFinanceTracker.Api.Models.RequestFilters;

public class GoalFilter : BaseFilter
{
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public DateTime? TargetDate { get; set; }
    public DateTime? FromTargetDate { get; set; }
    public DateTime? ToTargetDate { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}