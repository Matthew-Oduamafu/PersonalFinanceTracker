using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetEmployeeByIdAsync(string id);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync();
    IQueryable<Employee> GetEmployeesAsQueryable();
    Task<Employee> AddEmployeeAsync(Employee employee);
    Employee UpdateEmployeeAsync(Employee employee);
    public Task<bool> UpdateAsync(string id, 
        Expression<Func<SetPropertyCalls<Employee>, SetPropertyCalls<Employee>>> setPropertyExpression);
    Employee DeleteEmployeeAsync(Employee employee);
}