using System.Net.Http.Headers;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.BankTransactions;

public class DeleteTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly BankTransaction _bankTransaction1;
    private readonly BankTransaction _bankTransaction2;

    private readonly Bank _bank1;
    private readonly Bank _bank2;


    public DeleteTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task DeleteBankTransaction_Success_WhenAdminIsAuthorized()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var transactionId = _bankTransaction1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"bankTransactions/delete/{transactionId}");

        response.EnsureSuccessStatusCode();
        var deletedTransaction = await Context.BankTransactions.FindAsync(transactionId);
        Assert.Null(deletedTransaction);
    }

    [Fact]
    public async Task DeleteBankTransaction_Fails_WhenUserIsNotAdminTryOther()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var transactionId = _bankTransaction2.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"bankTransactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteBankTransaction_Fails_WhenUserIsNotAdminTryOwn()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var transactionId = _bankTransaction1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"bankTransactions/delete/{transactionId}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task DeleteBankTransaction_Fails_WhenTransactionNotFound()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var nonExistentTransactionId = Guid.NewGuid();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"bankTransactions/delete/{nonExistentTransactionId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBankTransaction_Fails_WhenNoAuthTokenProvided()
    {
        var transactionId = _bankTransaction1.Id;

        var response = await Client.DeleteAsync($"bankTransactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBankTransaction_Fails_WhenInvalidAuthTokenProvided()
    {
        var invalidAuthToken = "invalid_token";
        var transactionId = _bankTransaction1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidAuthToken);

        var response = await Client.DeleteAsync($"bankTransactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
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