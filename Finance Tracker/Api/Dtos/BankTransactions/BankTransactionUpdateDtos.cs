using Domain.BankTransactions;

namespace Api.Dtos.BankTransactions;

public record BankTransactionUpdateDto(decimal Amount)
{
    public static BankTransactionUpdateDto FromDomainModel(BankTransaction bankTransaction)
        => new(
            Amount: bankTransaction.Amount
        );
}