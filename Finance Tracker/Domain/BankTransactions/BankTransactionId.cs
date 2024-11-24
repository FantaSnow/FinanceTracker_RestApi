namespace Domain.BankTransactions;

public record BankTransactionId(Guid Value)
{
    public static BankTransactionId New() => new(Guid.NewGuid());
    public static BankTransactionId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}