using Api.Dtos;
using Domain.Transactions;
using FluentValidation;

namespace Api.Modules.Validators;

public class TransactionDtoValidator : AbstractValidator<TransactionDto>
{
    public TransactionDtoValidator()
    {
        RuleFor(x => x.Sum).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();

    }
}