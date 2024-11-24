using Api.Dtos.BankTransactions;
using FluentValidation;

namespace Api.Modules.Validators.BankTransactions;

public class BankTransactionDtoValidator : AbstractValidator<BankTransactionDto>
{
    public BankTransactionDtoValidator()
    {
    }
}