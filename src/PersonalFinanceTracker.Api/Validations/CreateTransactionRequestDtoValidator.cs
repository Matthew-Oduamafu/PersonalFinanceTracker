using FluentValidation;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Validations;

public class CreateTransactionRequestDtoValidator: AbstractValidator<CreateTransactionRequestDto>
{
    public CreateTransactionRequestDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.AccountId).NotEmpty().WithMessage("Account Id is required");
        RuleFor(x => x.Amount).NotEmpty().WithMessage("Amount is required");
        RuleFor(x => x.TransactionType).NotEmpty().WithMessage("Transaction Type is required")
            .Matches(@"^(Income|Expense)$")
            .WithMessage("Invalid Transaction Type. Must be Income or Expense.");
        RuleFor(x => x.TransactionDate).NotEmpty().WithMessage("Transaction Date is required");
    }
}