using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class AppUserRepository(ApplicationDbContext context) : IAppUserRepository
{
    public async Task<bool> AddAsync(AppUser appUser)
    {
        await context.Users.AddAsync(appUser);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(AppUser appUser)
    {
        context.Users.Update(appUser);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<AppUser, bool>> predicate,
        Expression<Func<SetPropertyCalls<AppUser>, SetPropertyCalls<AppUser>>> setPropertyExpression)
    {
        var res = await context.Users.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (user == null) return false;

        context.Users.Remove(user);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(AppUser appUser)
    {
        context.Users.Remove(appUser);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<AppUser?> GetAsync(string id)
    {
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<AppUser> GetAsQueryable()
    {
        return context.Users.AsNoTracking().AsQueryable();
    }
}