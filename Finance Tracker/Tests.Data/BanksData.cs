using Domain.Banks;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;

namespace Tests.Data;

public static class BanksData
{
    public static Bank Bank1(UserId userId)
        => Bank.New(BankId.New(), "name1", 100m, userId);
    public static Bank Bank2(UserId userId)
        => Bank.New(BankId.New(), "name2", 200m, userId);

}