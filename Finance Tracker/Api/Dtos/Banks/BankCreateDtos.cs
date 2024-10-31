using Domain.Banks;

namespace Api.Dtos.Banks;

public record BankCreateDto(string Name, decimal BalanceGoal)
{
    public static BankCreateDto FromDomainModel(Bank bank)
        => new(
            Name: bank.Name,
            BalanceGoal: bank.BalanceGoal
        );
}