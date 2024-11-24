using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.BankTransactions;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.BankTransactions;

public class CreateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    private readonly BankTransaction _bankTransaction1;
    private readonly BankTransaction _bankTransaction2;

    private readonly Bank _bank1;
    private readonly Bank _bank2;


    public CreateTests(IntegrationTestWebFactory factory) : base(factory)
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
    public async Task CreateBankTransaction_Success_WhenUserCreatesOwnTransaction()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
         Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
         
        var request = new BankTransactionCreateDto
        (
            Amount : 100
        );
        
        var response = await Client.PostAsJsonAsync($"bankTransactions/create/{_mainUser.Id.Value}/{_bank1.Id.Value}", request);

        response.EnsureSuccessStatusCode();
        var createdTransaction = await Context.BankTransactions
            .FirstOrDefaultAsync(t => t.UserId == _mainUser.Id && t.Amount == 100);

        Assert.NotNull(createdTransaction);
    }

    [Fact]
    public async Task CreateBankTransaction_Success_WhenAdminCreatesTransactionForAnotherUser()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new BankTransactionCreateDto
        (
            Amount : 200
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"bankTransactions/create/{_secondUser.Id}/{_bank2.Id}", request);

        response.EnsureSuccessStatusCode();
        var createdTransaction = await Context.BankTransactions
            .FirstOrDefaultAsync(t => t.UserId == _secondUser.Id && t.Amount == 200);

        Assert.NotNull(createdTransaction);
    }

    [Fact]
    public async Task CreateBankTransaction_Fails_WhenUserTriesToCreateTransactionForAnotherUser()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankTransactionCreateDto
        (
            Amount : 150
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"bankTransactions/create/{_secondUser.Id}/{_bank2.Id.Value}", request);

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateBankTransaction_Fails_WhenUserNotAuthorized()
    {
        var request = new BankTransactionCreateDto
        (
            Amount : 300
        );

        var response = await Client.PostAsJsonAsync($"bankTransactions/create/{_mainUser.Id.Value}/{_bank1.Id.Value}", request);

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