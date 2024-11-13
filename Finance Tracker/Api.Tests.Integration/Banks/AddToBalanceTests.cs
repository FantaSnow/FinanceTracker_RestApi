using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Categorys;
using Domain.Banks;
using Domain.Categorys;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Banks;

public class AddToBalanceTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public AddToBalanceTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
    [Fact]
    public async Task AddToBalance_Success_WhenUserAddsBalanceToOwnBank()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var balanceToAdd = 100m;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PutAsync($"banks/addToBalance/{_bank1.Id}/{balanceToAdd}", null);

        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Banks
            .FirstOrDefaultAsync(b => b.Id == _bank1.Id);

        Assert.NotNull(updatedBank);
        Assert.Equal(_bank1.Balance + balanceToAdd, updatedBank?.Balance);
    }

    [Fact]
    public async Task AddToBalance_Success_WhenAdminAddsBalanceToUserBank()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        decimal balanceToAdd = 200m;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PutAsync($"banks/addToBalance/{_bank2.Id}/{balanceToAdd}", null);

        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Banks
            .FirstOrDefaultAsync(b => b.Id == _bank2.Id);

        Assert.NotNull(updatedBank);
        Assert.Equal(_bank2.Balance + balanceToAdd, updatedBank?.Balance);
    }

    [Fact]
    public async Task AddToBalance_Fails_WhenUserTriesToAddBalanceToAnotherUsersBank()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        decimal balanceToAdd = 150m;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PutAsync($"banks/addToBalance/{_bank2.Id}/{balanceToAdd}", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AddToBalance_Fails_WhenUserNotAuthorized()
    {
        var balanceToAdd = 100m;

        var response = await Client.PutAsync($"banks/addToBalance/{_bank1.Id}/{balanceToAdd}", null);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Banks.AddRangeAsync(_bank1, _bank2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Banks.RemoveRange(Context.Banks);
        await SaveChangesAsync();
    }
}