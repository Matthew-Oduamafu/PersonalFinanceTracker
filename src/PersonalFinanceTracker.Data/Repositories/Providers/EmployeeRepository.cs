using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Data.Repositories.Providers;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Employee?> GetEmployeeByIdAsync(string id)
    {
        return await _dbContext.Employees.FindAsync(id);
    }

    public async Task<IReadOnlyList<Employee>> GetEmployeesAsync()
    {
        return await _dbContext.Employees.AsTracking().ToListAsync();
    }

    public IQueryable<Employee> GetEmployeesAsQueryable()
    {
        return _dbContext.Employees;
    }

    public async Task<Employee> AddEmployeeAsync(Employee employee)
    {
        await _dbContext.Employees.AddAsync(employee);
        return employee;
    }

    public Employee UpdateEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Update(employee);
        return employee;
    }

    public async Task<bool> UpdateAsync(string id,
        Expression<Func<SetPropertyCalls<Employee>, SetPropertyCalls<Employee>>> setPropertyExpression)
    {
        var res = await _dbContext.Employees
            .Where(x => id.Equals(x.Id))
            .ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public async Task<bool> UpdateV2Async(Expression<Func<Employee, bool>> predicate,
        Expression<Func<SetPropertyCalls<Employee>, SetPropertyCalls<Employee>>> setPropertyExpression)
    {
        var res = await _dbContext.Employees
            .Where(predicate)
            .ExecuteUpdateAsync(setPropertyExpression);
        return res > 0;
    }

    public Employee DeleteEmployeeAsync(Employee employee)
    {
        _dbContext.Employees.Remove(employee);
        return employee;
    }
}