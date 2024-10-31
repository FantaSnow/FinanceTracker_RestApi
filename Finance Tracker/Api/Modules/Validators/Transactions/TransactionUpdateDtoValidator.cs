using Api.Dtos.Transactions;
using FluentValidation;

namespace Api.Modules.Validators.Transactions;

public class TransactionUpdateDtoValidator : AbstractValidator<TransactionUpdateDto>
{
    public TransactionUpdateDtoValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}