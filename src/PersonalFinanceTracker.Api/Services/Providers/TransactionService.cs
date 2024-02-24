using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore.Query;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.DtoExtensions;
using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models;
using PersonalFinanceTracker.Data.Models.Dtos;
using PersonalFinanceTracker.Data.Repositories.Interfaces;

namespace PersonalFinanceTracker.Api.Services.Providers;

public class TransactionService : ITransactionService
{
    private readonly ILogger<TransactionService> _logger;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IAccountRepository _accountRepo;

    public TransactionService(ILogger<TransactionService> logger, 
        ITransactionRepository transactionRepo, 
        IAccountRepository accountRepo)
    {
        _logger = logger;
        _transactionRepo = transactionRepo;
        _accountRepo = accountRepo;
    }

    public async Task<IGenericApiResponse<TransactionResponseDto>> CreateTransactionAsync(CreateTransactionRequestDto request)
    {
        try
        {
            _logger.LogInformation("Creating transaction with request {@Request}", request.ToJson());
            
            /***
             * Check if account exists
             * If account does not exist, return 404
             * If account exists, add or subtract the amount from the account balance
             * Then create the transaction
             */
            
            var account = await _accountRepo.GetAsync(request.AccountId);
            if(account == null)
            {
                _logger.LogError("Account with id {AccountId} does not exist", request.AccountId);
                return GenericApiResponse<TransactionResponseDto>.Default.ToBadRequestApiResponse("Account does not exist");
            }
            
            account.Balance += TransactionType.Income.Equals(request.TransactionType) ? request.Amount : -request.Amount;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = request.CreatedBy;
            
            var updatedAccount = await _accountRepo.UpdateAsync(account);
            
            if(!updatedAccount)
            {
                _logger.LogError("Error updating account with request {@Request}", account.ToJson());
                return GenericApiResponse<TransactionResponseDto>.Default.ToFailedDependenciesApiResponse("Unable to update account balance");
            }
            
            var transaction = request.ToTransaction();
            var createdTransaction = await _transactionRepo.AddAsync(transaction);
            
            if(!createdTransaction)
            {
                _logger.LogError("Error creating transaction with request {@Request}", request.ToJson());
                return GenericApiResponse<TransactionResponseDto>.Default.ToFailedDependenciesApiResponse("Unable to create transaction");
            }
            
            var response = transaction.ToResponse();
            
            _logger.LogInformation("Transaction created with response {@Response}", response.ToJson());

            return response.ToCreatedApiResponse();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return GenericApiResponse<TransactionResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<TransactionResponseDto>> UpdateTransactionAsync(string id, UpdateTransactionRequestDto request)
    {
        try
        {
            _logger.LogInformation("Updating transaction with request {@Request}", request.ToJson());
            
            var account = await _accountRepo.GetAsync(request.AccountId);
            if(account == null)
            {
                _logger.LogError("Account with id {AccountId} does not exist", request.AccountId);
                return GenericApiResponse<TransactionResponseDto>.Default.ToNotFoundApiResponse("Account does not exist");
            }
            
            var transaction = await _transactionRepo.GetAsync(id);
            if(transaction == null)
            {
                _logger.LogError("Transaction with id {TransactionId} does not exist", id);
                return GenericApiResponse<TransactionResponseDto>.Default.ToNotFoundApiResponse("Transaction does not exist");
            }
            
            transaction = request.Adapt(transaction);
            
            Expression<Func<Transaction, bool>> predicate = x => id.Equals(x.Id);
            
            Expression<Func<SetPropertyCalls<Transaction>, SetPropertyCalls<Transaction>>> setPropertyExpression = x => x
                .SetProperty(y => y.Description, request.Description)
                .SetProperty(y => y.Amount, request.Amount)
                .SetProperty(y => y.TransactionDate, request.TransactionDate)
                .SetProperty(y => y.TransactionType, request.TransactionType)
                .SetProperty(y => y.UpdatedBy, request.UpdatedBy)
                .SetProperty(y => y.UpdatedAt, DateTime.UtcNow);
            
            var updatedTransaction = await _transactionRepo.UpdateAsync(predicate, setPropertyExpression);
            
            if(!updatedTransaction)
            {
                _logger.LogError("Error updating transaction with request {@Request}", request.ToJson());
                return GenericApiResponse<TransactionResponseDto>.Default.ToFailedDependenciesApiResponse();
            }
            
            var response = transaction.ToResponse();
            
            _logger.LogInformation("Transaction updated with response {@Response}", response.ToJson());

            return response.ToAcceptedApiResponse();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction");
            return GenericApiResponse<TransactionResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<TransactionResponseDto>> GetTransactionByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Getting transaction with id {Id}", id);
            
            var transaction = await _transactionRepo.GetAsync(id);
            if(transaction == null)
            {
                _logger.LogError("Transaction with id {TransactionId} does not exist", id);
                return GenericApiResponse<TransactionResponseDto>.Default.ToNotFoundApiResponse("Transaction does not exist");
            }
            
            var response = transaction.ToResponse();
            
            _logger.LogInformation("Transaction retrieved with response {@Response}", response.ToJson());

            return response.ToOkApiResponse();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction");
            return GenericApiResponse<TransactionResponseDto>.Default.ToInternalServerErrorApiResponse();
        }
    }

    public async Task<IGenericApiResponse<PagedList<TransactionResponseDto>>> GetTransactionsAsync(TransactionFilter filter)
    {
        try
        {
            _logger.LogInformation("Getting transactions with filter {@Filter}", filter.ToJson());
            
            var query = _transactionRepo.GetAsQueryable();
            
            query = ApplyQueryFilter(filter, query);

            query = "desc".Equals(filter.SortDir) ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);
            
            var mappedQuery = query.ProjectToType<TransactionResponseDto>();
            
            var pagedList = await mappedQuery.ToPagedList(filter.Page, filter.PageSize);
            
            _logger.LogInformation("Transactions retrieved with response {@Response}", pagedList.ToJson());
            
            return pagedList.ToOkApiResponse();
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions");
            return GenericApiResponse<PagedList<TransactionResponseDto>>.Default.ToInternalServerErrorApiResponse();
        }
    }

    private static IQueryable<Transaction> ApplyQueryFilter(TransactionFilter filter, IQueryable<Transaction> query)
    {
        if(!string.IsNullOrWhiteSpace(filter.AccountId))
        {
            query = query.Where(x => filter.AccountId.Equals(x.AccountId));
        }
            
        if(!string.IsNullOrWhiteSpace(filter.Description))
        {
            query = query.Where(x => filter.Description.Equals(x.Description));
        }
            
        if(!string.IsNullOrWhiteSpace(filter.TransactionType))
        {
            query = query.Where(x => filter.TransactionType.Equals(x.TransactionType));
        }
            
        if(filter.Amount.HasValue)
        {
            query = query.Where(x => filter.Amount.Value.Equals(x.Amount));
        }
            
        if(filter.FromTransactionDate.HasValue)
        {
            query = query.Where(x => x.TransactionDate >= filter.FromTransactionDate.Value);
        }
            
        if(filter.ToTransactionDate.HasValue)
        {
            query = query.Where(x => x.TransactionDate <= filter.ToTransactionDate.Value);
        }
            
        if(filter.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
        }
            
        if(filter.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);
        }

        return query;
    }
}