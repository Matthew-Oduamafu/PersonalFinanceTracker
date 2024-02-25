using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class GoalService(ILogger<GoalService> logger, IGoalRepository goalRepo) : IGoalService
{
    public async Task<IGenericApiResponse<GoalResponseDto>> CreateGoalAsync(CreateGoalRequestDto request)
    {
        try
        {
            logger.LogInformation("Creating goal with request {@Request}", request.ToJson());
            var goal = request.ToGoal();
            var createdGoal = await goalRepo.AddAsync(goal);

            if (!createdGoal)
            {
                logger.LogError("Error creating goal with request {@Request}", request.ToJson());
                return GenericApiResponse<GoalResponseDto>.Default.ToFailedDependenciesApiResponse("Unable to create goal");
            }
            
            var response = goal.ToResponse();
            
            logger.LogInformation("Successfully created goal with response {@Response}", response.ToJson());
            
            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating goal with request {@Request}", request.ToJson());
            return GenericApiResponse<GoalResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<GoalResponseDto>> UpdateGoalAsync(string id, UpdateGoalRequestDto request)
    {
        try
        {
            logger.LogInformation("Updating goal with request {@Request}", request.ToJson());
            
            var goal = await goalRepo.GetAsync(id);
            if (goal == null)
            {
                logger.LogError("Goal with id {Id} does not exist", id);
                return GenericApiResponse<GoalResponseDto>.Default.ToNotFoundApiResponse("Goal does not exist");
            }
            
            Expression<Func<Goal, bool>> predicate = x => id.Equals(x.Id);
            
            Expression<Func<SetPropertyCalls<Goal>, SetPropertyCalls<Goal>>> setPropertyExpression = x => x
                .SetProperty(y => y.Name, request.Name)
                .SetProperty(y => y.TargetAmount, request.TargetAmount)
                .SetProperty(y => y.TargetDate, request.TargetDate)
                .SetProperty(y => y.CurrentAmount, request.CurrentAmount)
                .SetProperty(y => y.UpdatedBy, request.UpdatedBy)
                .SetProperty(y => y.UpdatedAt, DateTime.UtcNow);
            
            goal = request.Adapt(goal);
            var updatedGoal = await goalRepo.UpdateAsync(predicate, setPropertyExpression);

            if (!updatedGoal)
            {
                logger.LogError("Error updating goal with request {@Request}", request.ToJson());
                return GenericApiResponse<GoalResponseDto>.Default.ToFailedDependenciesApiResponse("Unable to update goal");
            }
            
            var response = goal.ToResponse();
            
            logger.LogInformation("Successfully updated goal with response {@Response}", response.ToJson());
            
            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating goal with request {@Request}", request.ToJson());
            return GenericApiResponse<GoalResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<GoalResponseDto>> GetGoalByIdAsync(string id)
    {
        try
        {
            logger.LogInformation("Getting goal with id {Id}", id);
            var goal = await goalRepo.GetAsync(id);
            if (goal == null)
            {
                logger.LogError("Goal with id {Id} does not exist", id);
                return GenericApiResponse<GoalResponseDto>.Default.ToNotFoundApiResponse("Goal does not exist");
            }
            
            var response = goal.ToResponse();
            
            logger.LogInformation("Successfully retrieved goal with response {@Response}", response.ToJson());
            
            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting goal with id {Id}", id);
            return GenericApiResponse<GoalResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<PagedList<GoalResponseDto>>> GetGoalsAsync(GoalFilter filter)
    {
        try
        {
            logger.LogInformation("Getting goals with filter {@Filter}", filter.ToJson());
           
            var query = goalRepo.GetAsQueryable();
            
            if (!string.IsNullOrWhiteSpace(filter.UserId))
            {
                query = query.Where(x => filter.UserId.Equals(x.UserId));
            }
            
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(x => x.Name.Contains(filter.Name));
            }
            
            if (filter.TargetDate.HasValue)
            {
                query = query.Where(x => x.TargetDate.Date == filter.TargetDate.Value.Date);
            }
            
            if (filter.FromTargetDate.HasValue)
            {
                query = query.Where(x => x.TargetDate.Date >= filter.FromTargetDate.Value.Date);
            }
            
            if (filter.ToTargetDate.HasValue)
            {
                query = query.Where(x => x.TargetDate.Date <= filter.ToTargetDate.Value.Date);
            }
            
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
            }
            
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);
            }
            
            query = "desc".Equals(filter.SortDir) ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
            
            var mappedQuery = query.ProjectToType<GoalResponseDto>();
            
            var response = await mappedQuery.ToPagedList(filter.Page, filter.PageSize);
            
            logger.LogInformation("Successfully retrieved goals with response {@Response}", response.ToJson());
            
            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting goals with filter {@Filter}", filter.ToJson());
            return GenericApiResponse<PagedList<GoalResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<GoalResponseDto>> DeleteGoalAsync(string id)
    {
        try
        {
            logger.LogInformation("Deleting goal with id {Id}", id);
            
            var goal = await goalRepo.GetAsync(id);
            if (goal == null)
            {
                logger.LogError("Goal with id {Id} does not exist", id);
                return GenericApiResponse<GoalResponseDto>.Default.ToNotFoundApiResponse("Goal does not exist");
            }
            
            var deletedGoal = await goalRepo.DeleteAsync(goal);
            if (!deletedGoal)
            {
                logger.LogError("Error deleting goal with id {Id}", id);
                return GenericApiResponse<GoalResponseDto>.Default.ToFailedDependenciesApiResponse("Unable to delete goal");
            }
            
            logger.LogInformation("Successfully deleted goal with id {Id}", id);
            
            return goal.ToResponse().ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting goal with id {Id}", id);
            return GenericApiResponse<GoalResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }
}