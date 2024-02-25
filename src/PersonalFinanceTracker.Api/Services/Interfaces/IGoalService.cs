using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IGoalService
{
    Task<IGenericApiResponse<GoalResponseDto>> CreateGoalAsync(CreateGoalRequestDto request);
    Task<IGenericApiResponse<GoalResponseDto>> UpdateGoalAsync(string id, UpdateGoalRequestDto request);
    Task<IGenericApiResponse<GoalResponseDto>> GetGoalByIdAsync(string id);
    Task<IGenericApiResponse<PagedList<GoalResponseDto>>> GetGoalsAsync(GoalFilter filter);
    Task<IGenericApiResponse<GoalResponseDto>> DeleteGoalAsync(string id);
}