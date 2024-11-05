using Domain.Statistics;

namespace Api.Dtos.Statistics;

public record StatisicDto(
    decimal MinusSum,
    int MinusCountTransaction,
    int MinusCountCategory,
    decimal PlusSum,
    int PlusCountTransaction,
    int PlusCountCategory)
{
    public static StatisicDto FromDomainModel(Statistic statistic)
        => new(
            MinusSum: statistic.MinusSum,
            MinusCountTransaction: statistic.MinusCoutTransaction,
            MinusCountCategory: statistic.MinusCoutCategory,
            PlusSum: statistic.PlusSum,
            PlusCountTransaction: statistic.PlusCoutTransaction,
            PlusCountCategory: statistic.PlusCoutCategory
        );
}