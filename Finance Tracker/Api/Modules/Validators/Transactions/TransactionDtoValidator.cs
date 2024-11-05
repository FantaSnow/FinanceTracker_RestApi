using Api.Dtos.Transactions;
using FluentValidation;

namespace Api.Modules.Validators.Transactions;

public class TransactionDtoValidator : AbstractValidator<TransactionDto>
{
    public TransactionDtoValidator()
    {
    }
}