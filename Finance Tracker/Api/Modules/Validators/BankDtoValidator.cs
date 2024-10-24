
using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class BankDtoValidator : AbstractValidator<BankDto>
{
    public BankDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        
    }
}