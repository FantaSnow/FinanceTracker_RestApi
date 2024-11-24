using Domain.Banks;
using Domain.Users;

namespace Domain.BankTransactions;

public class BankTransaction
{
    public BankTransactionId Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public BankId BankId { get; private set; }
    public Bank? Bank { get; private set; }
    public UserId UserId { get; private set; }
    public User? User { get; private set; }
    
    private BankTransaction(BankTransactionId id, decimal amount, UserId userId, BankId bankId)
    {
        Id = id;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
        BankId = bankId;
        UserId = userId;
    }
    
    public static BankTransaction Create(BankTransactionId id, decimal amount, UserId userId, BankId bankId)
        => new(id, amount, userId, bankId);

    public void UpdateBalance(decimal amount)
    {
        Amount = amount;
    }
}