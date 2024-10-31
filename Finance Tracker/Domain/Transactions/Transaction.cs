using Domain.Categorys;
using Domain.Users;

namespace Domain.Transactions;

public class Transaction
{
    public TransactionId Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Sum { get; private set; }

    public UserId UserId { get; private set; }
    public User? User { get; private set; }

    public CategoryId? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Transaction(TransactionId id, decimal sum, UserId userId, CategoryId? categoryId)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        Sum = sum;
        UserId = userId;
        CategoryId = categoryId;
    }

    public static Transaction Create(TransactionId id, decimal sum, UserId userId, CategoryId? categoryId)
        => new(id, sum, userId, categoryId);

    public void UpdateDatails(decimal sum, CategoryId categoryId)
    {
        Sum = sum;
        CategoryId = categoryId;
    }
}