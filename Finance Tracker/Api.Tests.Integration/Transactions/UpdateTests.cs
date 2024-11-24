using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Banks;
using Api.Dtos.Transactions;
using Api.Dtos.Users;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Transactions;

public class UpdateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;

    private readonly Category _category1;
    private readonly Category _category2;


    public UpdateTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task UpdateTransaction_Success_WhenAdminUpdates()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new TransactionUpdateDto(
            Sum: 100m,
            CategoryId: _category1.Id.Value
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"transactions/update/{_transaction1.Id}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Transactions.FindAsync(_transaction1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank!.Sum.Should().Be(100m);
        updatedBank!.CategoryId.Value.Should().Be(_category1.Id.Value);

    }

    [Fact]
    public async Task UpdateTransaction_Success_WhenUserUpdatesOwn()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new TransactionUpdateDto(
            Sum: 100m,
            CategoryId: _category1.Id.Value
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"transactions/update/{_transaction1.Id}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Transactions.FindAsync(_transaction1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank!.Sum.Should().Be(100m);
        updatedBank.CategoryId!.Value.Should().Be(_category1.Id.Value);

    }

    [Fact]
    public async Task UpdateTransaction_Fails_WhenUserUpdatesOther()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new TransactionUpdateDto(
            Sum: 100m,
            CategoryId: _category1.Id.Value
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"transactions/update/{_transaction2.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);


    }
    [Fact]
    public async Task UpdateTransaction_Fails_WhenUserNotAuthorized()
    {
        // Arrange
        var request = new TransactionUpdateDto(
            Sum: 100m,
            CategoryId: _category1.Id.Value
        );
        // Act
        var response = await Client.PutAsJsonAsync($"transactions/update/{_transaction1.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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