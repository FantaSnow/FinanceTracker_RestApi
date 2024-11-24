using Application.Transactions.Commands;
using FluentValidation;

namespace Application.BankTransactions.Commands;

public class UpdateBankTransactionCommandValidator : AbstractValidator<UpdateBankTransactionCommand>
{
    public UpdateBankTransactionCommandValidator()
    {
        RuleFor(x => x.BankTransactionId).NotEmpty();
    }
}