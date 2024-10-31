using Domain.Statistics;

namespace Api.Dtos.Statistics;

public record StatisicCategoryDto(string Name, int CoutTransaction, decimal MinusSum, decimal PlusSum)
{
    public static StatisicCategoryDto FromDomainModel(StatisticCategory statistic)
        => new
        (
            Name: statistic.CategoryName,
            MinusSum: statistic.MinusSum,
            CoutTransaction: statistic.CoutTransaction,
            PlusSum: statistic.PlusSum
        );
}