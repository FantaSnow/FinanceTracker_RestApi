using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Banks;
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

public class CreateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public CreateTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
    [Fact]
    public async Task CreateBank_Fails_WhenUserIsNotAuthorized()
    {
        var request = new BankCreateDto(
            Name: "Unauthorized Bank", 
            BalanceGoal: 3000m
        );

        var response = await Client.PostAsJsonAsync($"banks/create/{_mainUser.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBank_Succeeds_WhenUserCreatesOwnBank()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankCreateDto(
            Name: "My New Bank", 
            BalanceGoal: 5000m
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"banks/create/{_mainUser.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdBank = await Context.Banks
            .FirstOrDefaultAsync(b => b.Name == "My New Bank");

        createdBank.Should().NotBeNull();
        createdBank.Name.Should().Be(request.Name);
        createdBank.BalanceGoal.Should().Be(request.BalanceGoal);
    }

    [Fact]
    public async Task CreateBank_Fails_WhenUserTriesToCreateBankForAnotherUser()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankCreateDto(
            Name: "Another User's Bank", 
            BalanceGoal: 10000m
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"banks/create/{_secondUser.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBank_Succeeds_WhenAdminCreatesBankForAnyUser()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new BankCreateDto(
            Name: "Admin Created Bank", 
            BalanceGoal: 10000m
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync($"banks/create/{_mainUser.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdBank = await Context.Banks
            .FirstOrDefaultAsync(b => b.Name == "Admin Created Bank");

        createdBank.Should().NotBeNull();
        createdBank.Name.Should().Be(request.Name);
        createdBank.BalanceGoal.Should().Be(request.BalanceGoal);
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