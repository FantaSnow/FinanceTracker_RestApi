namespace Domain.Banks;

public record BankId(Guid Value)
{
    public static BankId New() => new(Guid.NewGuid());
    public static BankId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}