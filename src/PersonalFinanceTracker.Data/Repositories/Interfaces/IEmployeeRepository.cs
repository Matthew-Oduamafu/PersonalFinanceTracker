using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetEmployeeByIdAsync(string id);
    Task<IReadOnlyList<Employee>> GetEmployeesAsync();
    IQueryable<Employee> GetEmployeesAsQueryable();
    Task<Employee> AddEmployeeAsync(Employee employee);
    Employee UpdateEmployeeAsync(Employee employee);
    Employee DeleteEmployeeAsync(Employee employee);
}