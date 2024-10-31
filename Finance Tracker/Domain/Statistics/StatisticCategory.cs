namespace Domain.Statistics;

public class StatisticCategory
{
    public string CategoryName { get; set; }
    public int CoutTransaction { get; set; }
    public decimal PlusSum { get; set; }
    public decimal MinusSum { get; set; }


    private StatisticCategory(string categoryName, decimal minusSum, decimal plusSum, int coutTransaction)
    {
        MinusSum = minusSum;
        PlusSum = plusSum;
        CoutTransaction = coutTransaction;
        CategoryName = categoryName;
    }

    public static StatisticCategory New(string categoryName, decimal minusSum, decimal plusSum, int coutTransaction)
        => new(categoryName, minusSum, plusSum, coutTransaction);

    public static StatisticCategory Empty()
        => new("null", 0, 0, 0);
}