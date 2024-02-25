using PersonalFinanceTracker.Data.Data.Entities;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Extensions.DtoExtensions;

public static class AccountDtoExtensions
{
    public static Account ToAccount(this CreateAccountRequestDto dto)
    {
        return new Account
        {
            UserId = dto.UserId,
            Name = dto.Name,
            Balance = dto.Balance,
            AccountType = dto.AccountType,
            CreatedBy = dto.CreatedBy
        };
    }

    public static Account ToAccount(this UpdateAccountRequestDto dto)
    {
        return new Account
        {
            UserId = dto.UserId,
            Name = dto.Name,
            Balance = dto.Balance,
            AccountType = dto.AccountType,
            CreatedBy = dto.UpdatedBy
        };
    }

    public static AccountResponseDto ToResponse(this Account obj)
    {
        return new AccountResponseDto
        {
            Id = obj.Id,
            UserId = obj.UserId,
            Name = obj.Name,
            Balance = obj.Balance,
            AccountType = obj.AccountType,
            CreatedBy = obj.CreatedBy,
            CreatedAt = obj.CreatedAt
        };
    }
}