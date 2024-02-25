using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.DtoExtensions;

public static class TransactionDtoExtensions
{
    public static Transaction ToTransaction(this CreateTransactionRequestDto dto)
    {
        return new Transaction
        {
            AccountId = dto.AccountId,
            Description = dto.Description,
            Amount = dto.Amount,
            TransactionDate = dto.TransactionDate,
            TransactionType = dto.TransactionType,
            CreatedBy = dto.CreatedBy
        };
    }

    public static Transaction ToTransaction(this UpdateTransactionRequestDto dto)
    {
        return new Transaction
        {
            AccountId = dto.AccountId,
            Description = dto.Description,
            Amount = dto.Amount,
            TransactionDate = dto.TransactionDate,
            TransactionType = dto.TransactionType,
            CreatedBy = dto.UpdatedBy
        };
    }

    public static TransactionResponseDto ToResponse(this Transaction obj)
    {
        return new TransactionResponseDto
        {
            Id = obj.Id,
            AccountId = obj.AccountId,
            Description = obj.Description,
            Amount = obj.Amount,
            TransactionDate = obj.TransactionDate,
            TransactionType = obj.TransactionType,
            CreatedBy = obj.CreatedBy,
            CreatedAt = obj.CreatedAt
        };
    }
}