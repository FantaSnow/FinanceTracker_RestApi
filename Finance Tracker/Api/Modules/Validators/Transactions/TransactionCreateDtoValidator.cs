using Api.Dtos.Transactions;
using FluentValidation;

namespace Api.Modules.Validators.Transactions;

public class TransactionCreateDtoValidator : AbstractValidator<TransactionCreateDto>
{
    public TransactionCreateDtoValidator()
    {
        RuleFor(x => x.Sum).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();

    }
}