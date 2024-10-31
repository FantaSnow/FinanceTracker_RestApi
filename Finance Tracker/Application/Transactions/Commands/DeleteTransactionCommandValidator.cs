using FluentValidation;

namespace Application.Transactions.Commands;

public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty();
    }
}