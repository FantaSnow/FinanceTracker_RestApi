using FluentValidation;

namespace Application.Banks.Commands;

public class CreateBankCommandValidator : AbstractValidator<CreateBankCommand>
{
    public CreateBankCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.BalanceGoal).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}