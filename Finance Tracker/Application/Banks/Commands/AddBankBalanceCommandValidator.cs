using FluentValidation;

namespace Application.Banks.Commands;

public class AddBankBalanceCommandValidator : AbstractValidator<AddBankBalanceCommand>
{
    public AddBankBalanceCommandValidator()
    {
        RuleFor(x => x.BankId).NotEmpty();
    }
}