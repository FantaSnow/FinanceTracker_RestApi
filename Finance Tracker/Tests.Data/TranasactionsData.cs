using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;

namespace Tests.Data;

public static class TranasctionsData
{
    public static Transaction Transactin1(UserId userId,CategoryId categoryId)
        => Transaction.Create(TransactionId.New(),10m,userId,categoryId);
    public static Transaction Transactin2(UserId userId,CategoryId categoryId)
        => Transaction.Create(TransactionId.New(),15m,userId,categoryId);

}