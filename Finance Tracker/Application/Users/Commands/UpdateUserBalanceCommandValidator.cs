using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserBalanceCommandValidator : AbstractValidator<UpdateUserBalanceCommand>
{
    public UpdateUserBalanceCommandValidator()
    {
        RuleFor(x => x.Balance).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}