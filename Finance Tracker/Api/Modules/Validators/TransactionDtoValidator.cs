using Api.Dtos;
using Api.Dtos.Transactions;
using Domain.Transactions;
using FluentValidation;

namespace Api.Modules.Validators;

public class TransactionDtoValidator : AbstractValidator<TransactionDto>
{
    public TransactionDtoValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}