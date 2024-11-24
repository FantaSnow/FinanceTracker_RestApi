using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;

namespace Tests.Data;

public static class BankTranasctionsData
{
    public static BankTransaction BankTransactin1(UserId userId,BankId bankId)
        => BankTransaction.Create(BankTransactionId.New(),10m,userId,bankId);
    
    public static BankTransaction BankTransactin2(UserId userId,BankId bankId)
        => BankTransaction.Create(BankTransactionId.New(),15m,userId,bankId);
}