using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IEmployeeService
{
    Task<IGenericApiResponse<EmployeeResponse>> GetEmployeeByIdAsync(string id);
    Task<IGenericApiResponse<IReadOnlyList<EmployeeResponse>>> GetEmployeesAsync();
    Task<IGenericApiResponse<PagedList<EmployeeResponse>>> GetEmployees(BaseFilter filter);
    Task<IGenericApiResponse<EmployeeResponse>> AddEmployeeAsync(EmployeeRequest employeeRequest);
    Task<IGenericApiResponse<EmployeeResponse>> UpdateEmployeeAsync(string id, EmployeeRequest employeeRequest);
    Task<IGenericApiResponse<EmployeeResponse>> DeleteEmployeeAsync(string id);
}