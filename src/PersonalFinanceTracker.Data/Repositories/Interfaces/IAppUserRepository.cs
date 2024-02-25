using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IAppUserRepository
{
    Task<bool> AddAsync(AppUser appUser);
    Task<bool> UpdateAsync(AppUser appUser);

    Task<bool> UpdateAsync(Expression<Func<AppUser, bool>> predicate,
        Expression<Func<SetPropertyCalls<AppUser>, SetPropertyCalls<AppUser>>> setPropertyExpression);

    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(AppUser appUser);
    Task<AppUser?> GetAsync(string id);
    IQueryable<AppUser> GetAsQueryable();
}