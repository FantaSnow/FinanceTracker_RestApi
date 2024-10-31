using Api.Dtos.Categorys;
using FluentValidation;

namespace Api.Modules.Validators.Categorys;

public class CategorUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategorUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}