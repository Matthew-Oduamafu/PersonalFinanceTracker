using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(
        Expression<Func<SetPropertyCalls<Account>, SetPropertyCalls<Account>>> setPropertyExpression)
    {
        // Expression<Func<SetPropertyCalls<Account>, SetPropertyCalls<Account>>> setPropertyExpression = calls =>
        //     calls
        //         .SetProperty(p => p.Name, "New Name")
        //         .SetProperty(p => p.UserId, "New Name")
        //         .SetProperty(p => p.Balance, 20.3m);

        var res = await _context.Accounts.ExecuteUpdateAsync(setPropertyExpression);

        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (account == null) return false;

        _context.Accounts.Remove(account);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Account?> GetAsync(string id)
    {
        return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Account> GetAsQueryable()
    {
        return _context.Accounts.AsNoTracking().AsQueryable();
    }
}