namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IPgRepository
{
    Task<int> SaveChangesAsync();
}