using FluentValidation;

namespace Application.Banks.Commands;

public class DeleteBankCommandValidator : AbstractValidator<DeleteBankCommand>
{
    public DeleteBankCommandValidator()
    {
        RuleFor(x => x.BankId).NotEmpty();
    }
}