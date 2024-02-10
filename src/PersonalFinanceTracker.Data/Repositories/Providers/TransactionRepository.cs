using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Transaction?> GetAsync(string id)
    {
        return await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Transaction> GetAsQueryable()
    {
        return _context.Transactions.AsNoTracking().AsQueryable();
    }
}