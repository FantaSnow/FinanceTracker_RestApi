using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Transactions;

public class DeleteTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;

    private readonly Category _category1;
    private readonly Category _category2;


    public DeleteTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();

        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();

        _transaction1 = TranasctionsData.Transactin1(_mainUser.Id, _category1.Id);
        _transaction2 = TranasctionsData.Transactin2(_secondUser.Id, _category2.Id);
    }
    
 [Fact]
    public async Task DeleteTransaction_Success_WhenAdminIsAuthorized()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var transactionId = _transaction1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"transactions/delete/{transactionId}");

        response.EnsureSuccessStatusCode();
        var deletedTransaction = await Context.Transactions.FindAsync(transactionId);
        Assert.Null(deletedTransaction);
    }

    [Fact]
    public async Task DeleteTransaction_Fails_WhenUserIsNotAdmin()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var transactionId = _transaction2.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"transactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_Fails_WhenTransactionNotFound()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var nonExistentTransactionId = Guid.NewGuid();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"transactions/delete/{nonExistentTransactionId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_Fails_WhenNoAuthTokenProvided()
    {
        var transactionId = _transaction1.Id;

        var response = await Client.DeleteAsync($"transactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_Fails_WhenInvalidAuthTokenProvided()
    {
        var invalidAuthToken = "invalid_token";
        var transactionId = _transaction1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidAuthToken);

        var response = await Client.DeleteAsync($"transactions/delete/{transactionId}");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Categorys.AddRangeAsync(_category1, _category2);
        await Context.Transactions.AddRangeAsync(_transaction1, _transaction2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Categorys.RemoveRange(Context.Categorys);
        Context.Transactions.RemoveRange(Context.Transactions);
        await SaveChangesAsync();
    }
}