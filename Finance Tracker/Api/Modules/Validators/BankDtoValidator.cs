
using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class BankDtoValidator : AbstractValidator<BankDto>
{
    public BankDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .MinimumLength(3);

        RuleFor(x => x.Balance)
            .NotEmpty();

        RuleFor(x => x.BalanceGoal)
            .NotEmpty();


    }
}