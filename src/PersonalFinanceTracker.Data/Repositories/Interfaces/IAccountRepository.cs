using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<bool> AddAsync(Account account);
    Task<bool> UpdateAsync(Account account);

    public Task<bool> UpdateAsync(
        Expression<Func<SetPropertyCalls<Account>, SetPropertyCalls<Account>>> setPropertyExpression);
    Task<bool> DeleteAsync(string id);
    Task<Account?> GetAsync(string id);
    IQueryable<Account> GetAsQueryable();
}