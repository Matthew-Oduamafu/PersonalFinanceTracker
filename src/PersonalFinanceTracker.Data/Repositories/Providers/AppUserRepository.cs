using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class AppUserRepository : IAppUserRepository
{
    private readonly ApplicationDbContext _context;

    public AppUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(AppUser appUser)
    {
        await _context.Users.AddAsync(appUser);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(AppUser appUser)
    {
        _context.Users.Update(appUser);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<AppUser, bool>> predicate,
        Expression<Func<SetPropertyCalls<AppUser>, SetPropertyCalls<AppUser>>> setPropertyExpression)
    {
        var res = await _context.Users.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (user == null) return false;

        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(AppUser appUser)
    {
        _context.Users.Remove(appUser);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<AppUser?> GetAsync(string id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<AppUser> GetAsQueryable()
    {
        return _context.Users.AsNoTracking().AsQueryable();
    }
}