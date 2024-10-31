using Api.Dtos.Statistics;
using FluentValidation;

namespace Api.Modules.Validators.Statistics;

public class StatisticDtoValidator : AbstractValidator<StatisicDto>
{
    public StatisticDtoValidator()
    {
    }
}