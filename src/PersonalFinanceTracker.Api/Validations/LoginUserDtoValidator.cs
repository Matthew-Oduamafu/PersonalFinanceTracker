using FluentValidation;
using PersonalFinanceTracker.Data.Models.Dtos;

namespace PersonalFinanceTracker.Api.Validations;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username).NotNull().EmailAddress().MinimumLength(1).MaximumLength(100);
        RuleFor(x => x.Password).NotNull().MinimumLength(1).MaximumLength(100);
    }
}