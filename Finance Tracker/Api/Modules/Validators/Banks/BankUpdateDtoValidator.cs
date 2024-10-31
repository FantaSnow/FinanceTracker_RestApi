using Api.Dtos.Banks;
using FluentValidation;

namespace Api.Modules.Validators.Banks;

public class BankUpdateDtoValidator : AbstractValidator<BankUpdateDto>
{
    public BankUpdateDtoValidator()
    {

    }
}