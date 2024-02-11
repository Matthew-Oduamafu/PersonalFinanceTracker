using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class GoalRepository : IGoalRepository
{
    private readonly ApplicationDbContext _context;

    public GoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Goal goal)
    {
        await _context.Goals.AddAsync(goal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Goal goal)
    {
        _context.Goals.Update(goal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<Goal, bool>> predicate,
        Expression<Func<SetPropertyCalls<Goal>, SetPropertyCalls<Goal>>> setPropertyExpression)
    {
        var res = await _context.Goals.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var goal = await _context.Goals.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (goal == null) return false;

        _context.Goals.Remove(goal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Goal goal)
    {
        _context.Goals.Remove(goal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Goal?> GetAsync(string id)
    {
        return await _context.Goals.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Goal> GetAsQueryable()
    {
        return _context.Goals.AsNoTracking().AsQueryable();
    }
}