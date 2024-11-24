using Domain.BankTransactions;

namespace Api.Dtos.BankTransactions;

public record BankTransactionCreateDto(decimal Amount)
{
    public static BankTransactionCreateDto FromDomainModel(BankTransaction bankTransaction)
        => new(
            Amount: bankTransaction.Amount
            );
}