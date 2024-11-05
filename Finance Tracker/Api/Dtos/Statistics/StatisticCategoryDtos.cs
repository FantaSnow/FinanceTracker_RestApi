using Domain.Statistics;

namespace Api.Dtos.Statistics;

public record StatisicCategoryDto(string Name, int CountTransaction, decimal MinusSum, decimal PlusSum)
{
    public static StatisicCategoryDto FromDomainModel(StatisticCategory statistic)
        => new
        (
            Name: statistic.CategoryName,
            MinusSum: statistic.MinusSum,
            CountTransaction: statistic.CoutTransaction,
            PlusSum: statistic.PlusSum
        );
}