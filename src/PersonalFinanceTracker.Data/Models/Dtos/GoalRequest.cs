using System.Text.Json.Serialization;

namespace PersonalFinanceTracker.Data.Models.Dtos;

public class GoalRequest
{
    [JsonIgnore] public string UserId { get; set; }

    public string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
}

public class CreateGoalRequestDto : GoalRequest
{
    [JsonIgnore] public string CreatedBy { get; set; }
}

public class UpdateGoalRequestDto : GoalRequest
{
    [JsonIgnore] public string UpdatedBy { get; set; }
}