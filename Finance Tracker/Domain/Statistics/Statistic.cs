namespace Domain.Statistics;

public class Statistic
{
    public decimal MinusSum { get; private set; }
    public int MinusCoutTransaction { get; private set; }
    public int MinusCoutCategory { get; private set; }
    public decimal PlusSum { get; private set; }
    public int PlusCoutTransaction { get; private set; }
    public int PlusCoutCategory { get; private set; }

    private Statistic(decimal minusSum, int minusCoutTransaction, int minusCoutCategory, decimal plusSum,
        int plusCoutTransaction, int plusCoutCategory)
    {
        MinusSum = minusSum;
        MinusCoutTransaction = minusCoutTransaction;
        MinusCoutCategory = minusCoutCategory;
        PlusSum = plusSum;
        PlusCoutTransaction = plusCoutTransaction;
        PlusCoutCategory = plusCoutCategory;
    }

    public static Statistic New(decimal minusSum, int minusCoutTransaction, int minusCoutCategory, decimal plusSum,
        int plusCoutTransaction, int plusCoutCategory)
        => new(minusSum, minusCoutTransaction, minusCoutCategory, plusSum, plusCoutTransaction, plusCoutCategory);

    public static Statistic Empty()
        => new(0, 0, 0, 0, 0, 0);
}