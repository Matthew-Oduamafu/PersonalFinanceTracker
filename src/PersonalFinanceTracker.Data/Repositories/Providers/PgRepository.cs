using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class PgRepository(ApplicationDbContext dbContext) : IPgRepository
{
    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }
}