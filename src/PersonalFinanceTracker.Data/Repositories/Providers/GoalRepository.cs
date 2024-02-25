using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class GoalRepository(ApplicationDbContext context) : IGoalRepository
{
    public async Task<bool> AddAsync(Goal goal)
    {
        await context.Goals.AddAsync(goal);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Goal goal)
    {
        context.Goals.Update(goal);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<Goal, bool>> predicate,
        Expression<Func<SetPropertyCalls<Goal>, SetPropertyCalls<Goal>>> setPropertyExpression)
    {
        var res = await context.Goals.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var goal = await context.Goals.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (goal == null) return false;

        context.Goals.Remove(goal);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Goal goal)
    {
        context.Goals.Remove(goal);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Goal?> GetAsync(string id)
    {
        return await context.Goals.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Goal> GetAsQueryable()
    {
        return context.Goals.AsNoTracking().AsQueryable();
    }
}