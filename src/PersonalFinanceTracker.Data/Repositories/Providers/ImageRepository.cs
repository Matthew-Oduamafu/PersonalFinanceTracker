using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class ImageRepository : IImageRepository
{
    private readonly ApplicationDbContext _context;

    public ImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddAsync(Image image)
    {
        await _context.Images.AddAsync(image);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Image image)
    {
        _context.Images.Update(image);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Expression<Func<Image, bool>> predicate,
        Expression<Func<SetPropertyCalls<Image>, SetPropertyCalls<Image>>> setPropertyExpression)
    {
        var res = await _context.Images.Where(predicate).ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var image = await _context.Images.FirstOrDefaultAsync(x => id.Equals(x.Id));
        if (image == null) return false;

        _context.Images.Remove(image);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Image image)
    {
        _context.Images.Remove(image);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Image?> GetAsync(string id)
    {
        return await _context.Images.AsNoTracking().FirstOrDefaultAsync(x => id.Equals(x.Id));
    }

    public IQueryable<Image> GetAsQueryable()
    {
        return _context.Images.AsNoTracking().AsQueryable();
    }
}