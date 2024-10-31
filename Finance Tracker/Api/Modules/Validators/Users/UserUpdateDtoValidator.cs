using Api.Dtos.Users;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}