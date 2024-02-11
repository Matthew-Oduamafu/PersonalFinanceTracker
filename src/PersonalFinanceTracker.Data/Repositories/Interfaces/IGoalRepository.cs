using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<bool> AddAsync(Goal goal);
    Task<bool> UpdateAsync(Goal goal);
    Task<bool> UpdateAsync(Expression<Func<Goal, bool>> predicate,
        Expression<Func<SetPropertyCalls<Goal>, SetPropertyCalls<Goal>>> setPropertyExpression);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(Goal goal);
    Task<Goal?> GetAsync(string id);
    IQueryable<Goal> GetAsQueryable();
}