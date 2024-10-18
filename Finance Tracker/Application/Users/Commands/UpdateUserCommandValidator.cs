using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(255).MinimumLength(2);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(2);
        RuleFor(x => x.UserId).NotEmpty();
    }
}