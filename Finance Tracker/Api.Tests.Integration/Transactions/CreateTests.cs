using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Transactions;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Transactions;

public class CreateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;

    private readonly Category _category1;
    private readonly Category _category2;


    public CreateTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task CreateTransaction_Success_WhenUserCreatesOwnTransaction()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
         Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
         
        var request = new TransactionCreateDto
        (
            Sum : 100,
            CategoryId : _category1.Id.Value
        );
        
        var response = await Client.PostAsJsonAsync($"transactions/create/{_mainUser.Id}", request);

        response.EnsureSuccessStatusCode();
        var createdTransaction = await Context.Transactions
            .FirstOrDefaultAsync(t => t.UserId == _mainUser.Id && t.CategoryId == _category1.Id && t.Sum == 100);

        Assert.NotNull(createdTransaction);
    }

    [Fact]
    public async Task CreateTransaction_Success_WhenAdminCreatesTransactionForAnotherUser()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new TransactionCreateDto
        (
            Sum : 200,
            CategoryId : _category2.Id.Value
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"transactions/create/{_secondUser.Id}", request);

        response.EnsureSuccessStatusCode();
        var createdTransaction = await Context.Transactions
            .FirstOrDefaultAsync(t => t.UserId == _secondUser.Id && t.CategoryId == _category2.Id && t.Sum == 200);

        Assert.NotNull(createdTransaction);
    }

    [Fact]
    public async Task CreateTransaction_Fails_WhenUserTriesToCreateTransactionForAnotherUser()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new TransactionCreateDto
        (
            Sum : 150,
            CategoryId : _category2.Id.Value
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"transactions/create/{_secondUser.Id}", request);

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_Fails_WhenUserNotAuthorized()
    {
        var request = new TransactionCreateDto
        (
            Sum : 300,
            CategoryId : _category1.Id.Value
        );

        var response = await Client.PostAsJsonAsync($"transactions/create/{_mainUser.Id}", request);

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