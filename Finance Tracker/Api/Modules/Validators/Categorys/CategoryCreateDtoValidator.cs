using Api.Dtos.Categorys;
using FluentValidation;

namespace Api.Modules.Validators.Categorys;

public class CategorCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategorCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}