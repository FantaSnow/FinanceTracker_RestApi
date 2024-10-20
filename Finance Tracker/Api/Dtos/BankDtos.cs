
using Domain.Banks;


namespace Api.Dtos;

public record BankDto(Guid? Id,string Name,decimal Balance, decimal BalanceGoal)
{
    public static BankDto FromDomainModel(Bank bank)
        => new(
            Id: bank.Id.Value,
            Name: bank.Name,
            Balance: bank.Balance,
            BalanceGoal:bank.BalanceGoal
            );
}