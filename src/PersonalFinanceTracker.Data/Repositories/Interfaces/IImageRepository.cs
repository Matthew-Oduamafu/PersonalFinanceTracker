using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IImageRepository
{
    Task<bool> AddAsync(Image image);
    Task<bool> UpdateAsync(Image image);
    Task<bool> UpdateAsync(Expression<Func<Image, bool>> predicate,
        Expression<Func<SetPropertyCalls<Image>, SetPropertyCalls<Image>>> setPropertyExpression);
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(Image image);
    Task<Image?> GetAsync(string id);
    IQueryable<Image> GetAsQueryable();
}