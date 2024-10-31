using Api.Dtos.Statistics;
using FluentValidation;

namespace Api.Modules.Validators.Statistics;

public class StatisticCategoryDtoValidator : AbstractValidator<StatisicCategoryDto>
{
    public StatisticCategoryDtoValidator()
    {
    }
}