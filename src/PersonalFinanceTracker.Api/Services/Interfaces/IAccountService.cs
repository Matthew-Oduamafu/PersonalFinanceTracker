using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface IAccountService
{
    Task<IGenericApiResponse<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto request);
    Task<IGenericApiResponse<AccountResponseDto>> UpdateAccountAsync(UpdateAccountRequestDto request);
    Task<IGenericApiResponse<AccountResponseDto>> DeleteAccountAsync(string id);
    Task<IGenericApiResponse<AccountResponseDto>> GetAccountAsync(string id);
    Task<IGenericApiResponse<PagedList<AccountResponseDto>>> GetAccountsAsync(AccountFilter filter);
}