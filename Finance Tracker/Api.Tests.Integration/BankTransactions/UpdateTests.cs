using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.BankTransactions;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.BankTransactions;

public class UpdateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly BankTransaction _bankTransaction1;
    private readonly BankTransaction _bankTransaction2;

    private readonly Bank _bank1;
    private readonly Bank _bank2;


    public UpdateTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task UpdateBankTransaction_Success_WhenAdminUpdates()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new BankTransactionUpdateDto(
            Amount: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"bankTransactions/update/{_bankTransaction1.Id.Value}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.BankTransactions.FindAsync(_bankTransaction1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank!.Amount.Should().Be(100m);
    }

    [Fact]
    public async Task UpdateBankTransaction_Success_WhenUserUpdatesOwn()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankTransactionUpdateDto(
            Amount: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"bankTransactions/update/{_bankTransaction1.Id.Value}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.BankTransactions.FindAsync(_bankTransaction1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank!.Amount.Should().Be(100m);
    }

    [Fact]
    public async Task UpdateBankTransaction_Fails_WhenUserUpdatesOther()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankTransactionUpdateDto(
            Amount: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"bankTransactions/update/{_bankTransaction2.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateBankTransaction_Fails_WhenUserNotAuthorized()
    {
        // Arrange
        var request = new BankTransactionUpdateDto(
            Amount: 100m
        );
        // Act
        var response = await Client.PutAsJsonAsync($"bankTransactions/update/{_bankTransaction1.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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