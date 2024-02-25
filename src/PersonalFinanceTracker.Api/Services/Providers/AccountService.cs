using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class AccountService(
    ILogger<AccountService> logger,
    IAccountRepository accountRepository,
    ILinkService linkService)
    : IAccountService
{
    public async Task<IGenericApiResponse<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto request)
    {
        try
        {
            logger.LogInformation("Creating account for user {UserId}, with request {Request}", request.UserId,
                JsonSerializer.Serialize(request));

            var accountExists = await accountRepository.ExistsAsync(request.UserId, request.Name, request.AccountType);

            if (accountExists)
            {
                logger.LogWarning("Account already exists for user {UserId}, with request {Request}", request.UserId,
                    JsonSerializer.Serialize(request));
                return GenericApiResponse<AccountResponseDto>.Default.ToBadRequestApiResponse(
                    "You already have an account with the same name and type");
            }

            var newAccount = request.ToAccount();
            var result = await accountRepository.AddAsync(newAccount);

            if (!result)
            {
                logger.LogWarning("Error creating account for user {UserId}, with request {Request}", request.UserId,
                    JsonSerializer.Serialize(request));
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            var response = newAccount.ToResponse();
            AddLinksForAccount(response);

            logger.LogInformation("Account created for user {UserId}, with response {Response}", request.UserId,
                JsonSerializer.Serialize(response));

            return response.ToCreatedApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> UpdateAccountAsync(string id,
        UpdateAccountRequestDto request)
    {
        try
        {
            logger.LogInformation("Updating account for user {UserId}, with request {Request}", request.UserId,
                JsonSerializer.Serialize(request));

            var accountExists = await accountRepository.GetAsync(id);

            if (accountExists is null)
            {
                logger.LogWarning("Account with id {Id} not found", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToNotFoundApiResponse();
            }

            Expression<Func<Account, bool>> predicate = x => id.Equals(x.Id);
            Expression<Func<SetPropertyCalls<Account>, SetPropertyCalls<Account>>> setPropertyExpression = x => x
                .SetProperty(y => y.AccountType, request.AccountType)
                .SetProperty(y => y.Name, request.Name)
                .SetProperty(y => y.UpdatedBy, request.UpdatedBy)
                .SetProperty(y => y.UpdatedAt, DateTime.UtcNow);

            var result = await accountRepository.UpdateAsync(predicate, setPropertyExpression);

            if (!result)
            {
                logger.LogWarning("Error updating account for user {UserId}, with request {Request}", request.UserId,
                    JsonSerializer.Serialize(request));
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            var response = accountExists.ToResponse();
            AddLinksForAccount(response);

            logger.LogInformation("Account updated for user {UserId}, with response {Response}", request.UserId,
                JsonSerializer.Serialize(response));

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> DeleteAccountAsync(string id)
    {
        try
        {
            logger.LogInformation("Deleting account with id {Id}", id);

            var account = await accountRepository.GetAsync(id);

            if (account == null)
            {
                logger.LogWarning("Account with id {Id} not found", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToNotFoundApiResponse();
            }

            var result = await accountRepository.DeleteAsync(account);

            if (!result)
            {
                logger.LogWarning("Error deleting account with id {Id}", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToFailedDependenciesApiResponse();
            }

            logger.LogInformation("Account deleted with id {Id}", id);

            return account.ToResponse().ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<AccountResponseDto>> GetAccountAsync(string id)
    {
        try
        {
            logger.LogInformation("Getting account with id {Id}", id);

            var account = await accountRepository.GetAsync(id);

            if (account == null)
            {
                logger.LogWarning("Account with id {Id} not found", id);
                return GenericApiResponse<AccountResponseDto>.Default.ToNotFoundApiResponse();
            }

            var response = account.ToResponse();
            AddLinksForAccount(response);

            logger.LogInformation("Account with id {Id} found", id);

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting account");
            return GenericApiResponse<AccountResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }


    public async Task<IGenericApiResponse<PagedList<AccountResponseDto>>> GetAccountsAsync(AccountFilter filter)
    {
        try
        {
            logger.LogInformation("Getting accounts with filter {Filter}", JsonSerializer.Serialize(filter));

            var query = accountRepository.GetAsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.AccountType))
                query = query.Where(x => x.AccountType == filter.AccountType);

            if (!string.IsNullOrWhiteSpace(filter.CreatedBy)) query = query.Where(x => x.CreatedBy == filter.CreatedBy);

            if (filter.FromDate.HasValue) query = query.Where(x => x.CreatedAt >= filter.FromDate);

            if (filter.ToDate.HasValue) query = query.Where(x => x.CreatedAt <= filter.ToDate);

            query = filter.SortDir == "desc"
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt);

            var responseQuery = query.Select(x => x.ToResponse());

            var response = await responseQuery.ToPagedList(filter.Page, filter.PageSize);

            logger.LogInformation("Accounts found with filter {Filter}", JsonSerializer.Serialize(filter));

            AddLinksForPagedAccounts(response, filter, linkService);

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting accounts");
            return GenericApiResponse<PagedList<AccountResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    private void AddLinksForAccount(AccountResponseDto response)
    {
        response?.Links.Add(
            linkService.GenerateLink("Get", new { id = response.Id }, "self", "GET"));
        response?.Links.Add(
            linkService.GenerateLink("Update", new { id = response.Id }, "update-account", "PUT"));
        response?.Links.Add(
            linkService.GenerateLink("Delete", new { id = response.Id }, "delete-account", "DELETE"));
    }

    private static void AddLinksForPagedAccounts(PagedList<AccountResponseDto> apiResponse, BaseFilter filter,
        ILinkService linkService)
    {
        if (apiResponse?.Items == null || !apiResponse.Items.Any()) return;

        apiResponse.Links.Add(
            linkService.GenerateLink("GetAll",
                new { filter.Page, filter.PageSize }, "self", "GET"));

        if (apiResponse.Page > 1)
            apiResponse.Links.Add(
                linkService.GenerateLink("GetAll",
                    new { Page = filter.Page - 1, filter.PageSize }, "previous-page", "GET"));

        if (apiResponse.Page < apiResponse.TotalPages)
            apiResponse.Links.Add(
                linkService.GenerateLink("GetAll",
                    new { Page = filter.Page + 1, filter.PageSize }, "next-page", "GET"));
    }
}