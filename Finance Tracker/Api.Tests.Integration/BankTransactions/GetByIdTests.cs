using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.BankTransactions;

public class GetByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly BankTransaction _bankTransaction1;
    private readonly BankTransaction _bankTransaction2;

    private readonly Bank _bank1;
    private readonly Bank _bank2;


    public GetByIdTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task GetById_Success_WhenTransactionExists()
    {
        // Arrange
        var transactionId = _bankTransaction1.Id;

        // Act
        var response = await Client.GetAsync($"bankTransactions/getById/{transactionId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transactionInDb = await Context.BankTransactions.FindAsync(transactionId);
        Assert.NotNull(transactionInDb);
        Assert.Equal(_bankTransaction1.Id, transactionInDb.Id);
    }

    [Fact]
    public async Task GetById_Fails_WhenTransactionNotFound()
    {
        // Arrange
        var nonExistentTransactionId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"bankTransactions/getById/{nonExistentTransactionId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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