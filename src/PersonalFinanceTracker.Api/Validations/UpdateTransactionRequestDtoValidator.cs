using FluentValidation;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Validations;

public class UpdateTransactionRequestDtoValidator: AbstractValidator<UpdateTransactionRequestDto>
{
    public UpdateTransactionRequestDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("Account Id is required");
        RuleFor(x => x.Amount).NotEmpty().WithMessage("Amount is required");
        RuleFor(x => x.TransactionType).NotEmpty().WithMessage("Transaction Type is required");
        RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("Transaction Date is required");
    }
}