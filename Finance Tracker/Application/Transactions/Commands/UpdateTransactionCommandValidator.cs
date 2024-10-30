using Application.Banks.Commands;
using FluentValidation;

namespace Application.Transactions.Commands;

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId).NotEmpty();
  
    }
}