using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class CategorDtoValidator : AbstractValidator<CategoryDto>
{
    public CategorDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        
    }

}