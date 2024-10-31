using Api.Dtos.Users;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}