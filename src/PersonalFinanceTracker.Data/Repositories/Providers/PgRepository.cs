using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class PgRepository : IPgRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PgRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}