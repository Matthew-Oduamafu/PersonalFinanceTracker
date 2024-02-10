using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(string id);
    Task<Transaction?> GetAsync(string id);
    IQueryable<Transaction> GetAsQueryable();
}