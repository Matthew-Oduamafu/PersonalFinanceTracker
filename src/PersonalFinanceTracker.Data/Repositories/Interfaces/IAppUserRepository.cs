using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IAppUserRepository
{
    Task<bool> CreateAsync(AppUser user);
    Task<AppUser?> GetAsync(string id);
    Task<AppUser?> GetByUseNameAndEmailAsync(string userName, string email);
    Task<AppUser?> GetByUseNameAsync(string userName);
    IQueryable<AppUser> GetAsQueryable();
    Task<bool> UpdateAsync(AppUser user);
    Task<bool> DeleteAsync(string id);
}