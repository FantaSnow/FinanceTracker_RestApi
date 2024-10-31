using FluentValidation;

namespace Application.Statistics.Commands;

public class GetByTimeAndCategoryCommandValidator : AbstractValidator<GetByTimeAndCategoryCommand>
{
    public GetByTimeAndCategoryCommandValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}