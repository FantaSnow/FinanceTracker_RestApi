using Api.Dtos.BankTransactions;
using FluentValidation;

namespace Api.Modules.Validators.BankTransactions;

public class BankTransactionCreateDtoValidator : AbstractValidator<BankTransactionCreateDto>
{
    public BankTransactionCreateDtoValidator()
    {
        RuleFor(x => x.Amount).NotEmpty();

    }
}