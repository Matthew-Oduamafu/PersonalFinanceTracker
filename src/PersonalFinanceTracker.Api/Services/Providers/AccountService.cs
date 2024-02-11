using System.Text.Json;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly ILinkService _linkService;

    public AccountService(ILogger<AccountService> logger, IAccountRepository accountRepository,
        ILinkService linkService)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _linkService = linkService;
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto request)
    {
        try
        {
            _logger.LogInformation("Creating account for user {UserId}, with request {Request}", request.UserId,
                JsonSerializer.Serialize(request));

            var newAccount = request.ToAccount();
            var result = await _accountRepository.AddAsync(newAccount);

            if (!result)
            {
                _logger.LogWarning("Error creating account for user {UserId}, with request {Request}", request.UserId,
                    JsonSerializer.Serialize(request));
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            var response = newAccount.ToResponse();
            AddLinksForAccount(response);

            _logger.LogInformation("Account created for user {UserId}, with response {Response}", request.UserId,
                JsonSerializer.Serialize(response));

            return response.ToCreatedApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> UpdateAccountAsync(UpdateAccountRequestDto request)
    {
        try
        {
            _logger.LogInformation("Updating account for user {UserId}, with request {Request}", request.UserId,
                JsonSerializer.Serialize(request));

            var account = request.ToAccount();
            var result = await _accountRepository.UpdateAsync(account);

            if (!result)
            {
                _logger.LogWarning("Error updating account for user {UserId}, with request {Request}", request.UserId,
                    JsonSerializer.Serialize(request));
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            var response = account.ToResponse();
            AddLinksForAccount(response);

            _logger.LogInformation("Account updated for user {UserId}, with response {Response}", request.UserId,
                JsonSerializer.Serialize(response));

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> DeleteAccountAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deleting account with id {Id}", id);

            var account = await _accountRepository.GetAsync(id);

            if (account == null)
            {
                _logger.LogWarning("Account with id {Id} not found", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToNotFoundApiResponse();
            }

            var result = await _accountRepository.DeleteAsync(account);

            if (!result)
            {
                _logger.LogWarning("Error deleting account with id {Id}", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            _logger.LogInformation("Account deleted with id {Id}", id);

            return account.ToResponse().ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> GetAccountAsync(string id)
    {
        try
        {
            _logger.LogInformation("Getting account with id {Id}", id);

            var account = await _accountRepository.GetAsync(id);

            if (account == null)
            {
                _logger.LogWarning("Account with id {Id} not found", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToNotFoundApiResponse();
            }

            var response = account.ToResponse();
            AddLinksForAccount(response);

            _logger.LogInformation("Account with id {Id} found", id);

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<PagedList<AccountResponseDto>>> GetAccountsAsync(AccountFilter filter)
    {
        try
        {
            _logger.LogInformation("Getting accounts with filter {Filter}", JsonSerializer.Serialize(filter));

            var query = _accountRepository.GetAsQueryable();
            
            if (!string.IsNullOrWhiteSpace(filter.AccountType))
            {
                query = query.Where(x => x.AccountType == filter.AccountType);
            }
            
            if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            {
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);
            }
            
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.FromDate);
            }
            
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= filter.ToDate);
            }
            
            query = filter.SortDir == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);

            var responseQuery = query.Select(x => x.ToResponse());
            
            var response = await responseQuery.ToPagedList(filter.Page, filter.PageSize);
            
            _logger.LogInformation("Accounts found with filter {Filter}", JsonSerializer.Serialize(filter));

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts");
            return GenericApiResponse<PagedList<AccountResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    private void AddLinksForAccount(AccountResponseDto response)
    {
        response?.Links.Add(
            _linkService.GenerateLink("Get", new { id = response.Id }, "self", "GET"));
        response?.Links.Add(
            _linkService.GenerateLink("Update", new { id = response.Id }, "update-account", "PUT"));
        response?.Links.Add(
            _linkService.GenerateLink("Delete", new { id = response.Id }, "delete-account", "DELETE"));
    }
}