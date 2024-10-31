using Domain.Banks;

namespace Api.Dtos.Banks;

public record BankUpdateDto(string Name, decimal BalanceGoal)
{
    public static BankUpdateDto FromDomainModel(Bank bank)
        => new(
            Name: bank.Name,
            BalanceGoal: bank.BalanceGoal
        );
}