using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class ImageRepository(ApplicationDbContext context) : IImageRepository
{
    public async Task<bool> AddAsync(Image image)
    {
        await context.Images.AddAsync(image);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Image image)
    {
        context.Images.Update(image);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<Image, bool>> predicate,
        Expression<Func<SetPropertyCalls<Image>, SetPropertyCalls<Image>>> setPropertyExpression)
    {
        var res = await context.Images.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var image = await context.Images.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (image == null) return false;

        context.Images.Remove(image);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Image image)
    {
        context.Images.Remove(image);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Image?> GetAsync(string id)
    {
        return await context.Images.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Image> GetAsQueryable()
    {
        return context.Images.AsNoTracking().AsQueryable();
    }
}