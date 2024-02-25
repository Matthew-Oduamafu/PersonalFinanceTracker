using FluentValidation;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Validations;

public class CreateAccountRequestDtoValidator : AbstractValidator<CreateAccountRequestDto>
{
    public CreateAccountRequestDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Balance)
            .NotNull().NotEmpty()
            .WithMessage("Balance is required")
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AccountType)
            .NotEmpty().NotNull()
            .WithMessage("Account Type is required")
            .Matches(@"^(Savings|Current|FixedDeposit|CreditCard)$")
            .WithMessage("Invalid Account Type. Must be Savings, Current, FixedDeposit, or CreditCard.");
    }
}