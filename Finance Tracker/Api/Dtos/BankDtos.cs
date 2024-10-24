
using Domain.Banks;
using Domain.Users;


namespace Api.Dtos;

public record BankDto(Guid? BankId,string Name,decimal Balance, decimal BalanceGoal, Guid UserId)
{
    public static BankDto FromDomainModel(Bank bank)
        => new(
            BankId: bank.Id.Value,
            Name: bank.Name,
            Balance: bank.Balance,
            BalanceGoal:bank.BalanceGoal,
            UserId: bank.UserId.Value
            );
}