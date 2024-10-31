using Api.Dtos.Banks;
using FluentValidation;

namespace Api.Modules.Validators.Banks;

public class BankDtoCreateValidator : AbstractValidator<BankCreateDto>
{
    public BankDtoCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.BalanceGoal)
            .NotEmpty();
    }
}