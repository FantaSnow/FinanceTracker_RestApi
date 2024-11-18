using FluentValidation;

namespace Application.Statistics.Commands;

public class GetByTimeAndCategoryForAllCommandValidator : AbstractValidator<GetByTimeAndCategoryForAllCommand>
{
    public GetByTimeAndCategoryForAllCommandValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}