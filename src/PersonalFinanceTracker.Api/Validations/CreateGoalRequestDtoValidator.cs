using FluentValidation;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Validations;

public class CreateGoalRequestDtoValidator : AbstractValidator<CreateGoalRequestDto>
{
    public CreateGoalRequestDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.TargetAmount).GreaterThan(0).WithMessage("Target amount must be greater than 0");
        RuleFor(x => x.TargetDate).NotEmpty().WithMessage("Target date is required");
    }
}