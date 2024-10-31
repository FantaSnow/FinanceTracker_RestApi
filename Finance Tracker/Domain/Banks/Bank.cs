using Domain.Users;

namespace Domain.Banks;

public class Bank
{
    public BankId Id { get; private set; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }
    public decimal BalanceGoal { get; private set; }
    public UserId UserId { get; private set; }
    public User? User { get; private set; }

    private Bank(BankId id, string name, decimal balanceGoal, UserId userId)
    {
        Id = id;
        Name = name;
        Balance = 0m;
        BalanceGoal = balanceGoal;
        UserId = userId;
    }

    public static Bank New(BankId id, string name, decimal balanceGoal, UserId userId)
        => new(id, name, balanceGoal, userId);

    public void UpdateDatails(string name, decimal balanceGoal)
    {
        Name = name;
        BalanceGoal = balanceGoal;
    }

    public void AddToBalance(decimal balance)
    {
        Balance += balance;
    }
}