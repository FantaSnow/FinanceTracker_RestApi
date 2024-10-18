namespace Domain.Transactions;

public record TransactionId(Guid Value)
{
    public static TransactionId New() => new(Guid.NewGuid());
    public static TransactionId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}