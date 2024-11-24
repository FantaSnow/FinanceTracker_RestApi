using Api.Dtos.BankTransactions;
using Api.Dtos.Transactions;
using FluentValidation;

namespace Api.Modules.Validators.BankTransactions;

public class BankTransactionUpdateDtoValidator : AbstractValidator<BankTransactionUpdateDto>
{
    public BankTransactionUpdateDtoValidator()
    {
        RuleFor(x => x.Amount).NotEmpty();
    }
}