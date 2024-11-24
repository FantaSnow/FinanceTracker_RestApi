using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.BankTransactions;

public class GetAllByUserTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly BankTransaction _bankTransaction1;
    private readonly BankTransaction _bankTransaction2;

    private readonly Bank _bank1;
    private readonly Bank _bank2;

    public GetAllByUserTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();

        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);

        _bankTransaction1 = BankTranasctionsData.BankTransactin1(_mainUser.Id, _bank1.Id);
        _bankTransaction2 = BankTranasctionsData.BankTransactin2(_secondUser.Id, _bank2.Id);
    }
    [Fact]
    public async Task GetAllByUser_Success_WhenTransactionsExist()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"bankTransactions/getAllByUser/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var transactionsInDb = await Context.BankTransactions
            .Where(t => t.UserId == userId)
            .ToListAsync();

        Assert.NotNull(transactionsInDb);
        Assert.Contains(transactionsInDb, t => t.Id == _bankTransaction1.Id);
        Assert.DoesNotContain(transactionsInDb, t => t.Id == _bankTransaction2.Id); 
    }
    

    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Banks.AddRangeAsync(_bank1, _bank2);
        await Context.BankTransactions.AddRangeAsync(_bankTransaction1, _bankTransaction2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Banks.RemoveRange(Context.Banks);
        Context.BankTransactions.RemoveRange(Context.BankTransactions);
        await SaveChangesAsync();
    }
}