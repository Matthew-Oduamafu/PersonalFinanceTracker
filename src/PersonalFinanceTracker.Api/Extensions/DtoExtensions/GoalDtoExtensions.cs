using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.DtoExtensions;

public static class GoalDtoExtensions
{
    public static Goal ToGoal(this CreateGoalRequestDto dto)
    {
        return new Goal
        {
            UserId = dto.UserId,
            Name = dto.Name,
            TargetAmount = dto.TargetAmount,
            TargetDate = dto.TargetDate,
            CurrentAmount = dto.CurrentAmount,
            CreatedBy = dto.CreatedBy
        };
    }
    public static Goal ToGoal(this UpdateGoalRequestDto dto)
    {
        return new Goal
        {
            UserId = dto.UserId,
            Name = dto.Name,
            TargetAmount = dto.TargetAmount,
            TargetDate = dto.TargetDate,
            CurrentAmount = dto.CurrentAmount,
            CreatedBy = dto.UpdatedBy
        };
    }
    
    public static GoalResponseDto ToResponse(this Goal goal)
    {
        return new GoalResponseDto
        {
            Id = goal.Id,
            UserId = goal.UserId,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            TargetDate = goal.TargetDate,
            CurrentAmount = goal.CurrentAmount,
            CreatedBy = goal.CreatedBy,
            CreatedAt = goal.CreatedAt
        };
    }
}