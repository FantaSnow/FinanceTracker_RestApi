using Application.Transactions.Commands;
using FluentValidation;

namespace Application.BankTransactions.Commands;

public class DeleteBankTransactionCommandValidator : AbstractValidator<DeleteBankTransactionCommand>
{
    public DeleteBankTransactionCommandValidator()
    {
        RuleFor(x => x.BankTransactionId).NotEmpty();
    }
}