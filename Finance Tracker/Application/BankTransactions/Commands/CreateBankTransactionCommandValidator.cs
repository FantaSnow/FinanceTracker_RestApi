using Application.Transactions.Commands;
using FluentValidation;

namespace Application.BankTransactions.Commands;

public class CreateBankTransactionCommandValidator : AbstractValidator<CreateBankTransactionCommand>
{
    public CreateBankTransactionCommandValidator()
    {
        RuleFor(x => x.Sum).NotEmpty();
        RuleFor(x => x.BankId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}