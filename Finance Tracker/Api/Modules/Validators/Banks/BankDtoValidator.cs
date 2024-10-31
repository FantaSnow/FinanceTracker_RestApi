using Api.Dtos.Banks;
using FluentValidation;

namespace Api.Modules.Validators.Banks;

public class BankDtoValidator : AbstractValidator<BankDto>
{
    public BankDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}