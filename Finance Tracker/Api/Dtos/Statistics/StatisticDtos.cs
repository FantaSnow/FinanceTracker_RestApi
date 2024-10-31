using Domain.Statistics;

namespace Api.Dtos.Statistics;

public record StatisicDto(decimal MinusSum, int MinusCoutTransaction, int MinusCoutCategory,decimal PlusSum, int PlusCoutTransaction, int PlusCoutCategory)
{
    public static StatisicDto FromDomainModel(Statistic statistic)

        => new(
            MinusSum: statistic.MinusSum,
            MinusCoutTransaction: statistic.MinusCoutTransaction,
            MinusCoutCategory: statistic.MinusCoutCategory,
            PlusSum: statistic.PlusSum,
            PlusCoutTransaction: statistic.PlusCoutTransaction,
            PlusCoutCategory: statistic.PlusCoutCategory
            );

}