using Api.Dtos;
using Api.Dtos.Categorys;
using FluentValidation;

namespace Api.Modules.Validators;

public class CategorDtoValidator : AbstractValidator<CategoryDto>
{
    public CategorDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        
    }

}