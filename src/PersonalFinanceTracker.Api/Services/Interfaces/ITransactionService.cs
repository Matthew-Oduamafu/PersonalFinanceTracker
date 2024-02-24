using PersonalFinanceTracker.Api.Models;
using PersonalFinanceTracker.Api.Models.RequestFilters;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<IGenericApiResponse<TransactionResponseDto>> CreateTransactionAsync(CreateTransactionRequestDto request);
    Task<IGenericApiResponse<TransactionResponseDto>> UpdateTransactionAsync(string id, UpdateTransactionRequestDto request);
    Task<IGenericApiResponse<TransactionResponseDto>> GetTransactionByIdAsync(string id);
    Task<IGenericApiResponse<PagedList<TransactionResponseDto>>> GetTransactionsAsync(TransactionFilter filter);
}