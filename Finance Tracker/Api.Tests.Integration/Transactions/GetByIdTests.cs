using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Transactions;

public class GetByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;

    private readonly Category _category1;
    private readonly Category _category2;


    public GetByIdTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task GetById_Success_WhenTransactionExists()
    {
        // Arrange
        var transactionId = _transaction1.Id;

        // Act
        var response = await Client.GetAsync($"transactions/getbyid/{transactionId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transactionInDb = await Context.Transactions.FindAsync(transactionId);
        Assert.NotNull(transactionInDb);
        Assert.Equal(_transaction1.Id, transactionInDb?.Id);
    }

    [Fact]
    public async Task GetById_Fails_WhenTransactionNotFound()
    {
        // Arrange
        var nonExistentTransactionId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"transactions/getbyid/{nonExistentTransactionId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
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