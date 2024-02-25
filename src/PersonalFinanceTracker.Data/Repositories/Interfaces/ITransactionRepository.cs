using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<bool> AddAsync(Transaction transaction);
    Task<bool> UpdateAsync(Transaction transaction);

    Task<bool> UpdateAsync(Expression<Func<Transaction, bool>> predicate,
        Expression<Func<SetPropertyCalls<Transaction>, SetPropertyCalls<Transaction>>> setPropertyExpression);

    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(Transaction transaction);
    Task<Transaction?> GetAsync(string id);
    IQueryable<Transaction> GetAsQueryable();
}