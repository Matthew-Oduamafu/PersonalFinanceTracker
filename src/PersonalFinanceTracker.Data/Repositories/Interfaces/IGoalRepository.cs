using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<bool> AddAsync(Goal goal);
    Task<bool> UpdateAsync(Goal goal);
    Task<bool> DeleteAsync(string id);
    Task<Goal?> GetAsync(string id);
    IQueryable<Goal> GetAsQueryable();
}