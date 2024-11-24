using Api.Dtos.Banks;
using Api.Dtos.Users;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Transactions;

namespace Api.Dtos.BankTransactions;

public record BankTransactionDto(
    Guid? Id,
    decimal Amount,
    DateTime CreatedAt,
    Guid UserId,
    UserDto? User,
    Guid BankId,
    BankDto? Bank)
{
    public static BankTransactionDto FromDomainModel(BankTransaction bankTransaction)
        => new(
            Id: bankTransaction.Id.Value,
            CreatedAt: bankTransaction.CreatedAt,
            Amount: bankTransaction.Amount,
            UserId: bankTransaction.UserId.Value,
            User: bankTransaction.User == null ? null : UserDto.FromDomainModel(bankTransaction.User),
            BankId: bankTransaction.UserId.Value,
            Bank: bankTransaction.Bank == null ? null : BankDto.FromDomainModel(bankTransaction.Bank)
        );
}