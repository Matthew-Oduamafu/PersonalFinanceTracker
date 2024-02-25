using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
{
    public async Task<bool> AddAsync(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<Transaction, bool>> predicate,
        Expression<Func<SetPropertyCalls<Transaction>, SetPropertyCalls<Transaction>>> setPropertyExpression)
    {
        var res = await context.Transactions.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var transaction = await context.Transactions.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (transaction == null) return false;

        context.Transactions.Remove(transaction);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Transaction transaction)
    {
        context.Transactions.Remove(transaction);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Transaction?> GetAsync(string id)
    {
        return await context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Transaction> GetAsQueryable()
    {
        return context.Transactions.AsNoTracking().AsQueryable();
    }
}