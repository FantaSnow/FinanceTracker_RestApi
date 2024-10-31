using FluentValidation;

namespace Application.Statistics.Commands;

public class GetByTimeForCategoryCommandValidator : AbstractValidator<GetByTimeForCategoryCommand>
{
    public GetByTimeForCategoryCommandValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}