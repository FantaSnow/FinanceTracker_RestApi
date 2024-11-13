using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Banks;
using Api.Dtos.Categorys;
using Domain.Banks;
using Domain.Categorys;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Banks;

public class UpdateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public UpdateTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
    [Fact]
    public async Task UpdateBank_Success_WhenAdminUpdatesBank()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new BankUpdateDto(
            Name: "User Updated Bank Name",
            BalanceGoal: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"banks/update/{_bank1.Id}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Banks.FindAsync(_bank1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank.Name.Should().Be("User Updated Bank Name");
        updatedBank.BalanceGoal.Should().Be(100m);

    }

    [Fact]
    public async Task UpdateBank_Success_WhenUserUpdatesOwnBank()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankUpdateDto(
            Name: "User Updated Bank Name",
            BalanceGoal: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"banks/update/{_bank1.Id}", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedBank = await Context.Banks.FindAsync(_bank1.Id);
        updatedBank.Should().NotBeNull();
        updatedBank.Name.Should().Be("User Updated Bank Name");
        updatedBank.BalanceGoal.Should().Be(100m);

    }

    [Fact]
    public async Task UpdateBank_Success_WhenUserUpdatesOtherBank()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new BankUpdateDto(
            Name: "User Updated Bank Name",
            BalanceGoal: 100m
        );
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"banks/update/{_bank2.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);


    }
    [Fact]
    public async Task UpdateBank_Fails_WhenUserNotAuthorized()
    {
        // Arrange
        var request = new BankUpdateDto(
            Name: "User Updated Bank Name",
            BalanceGoal: 100m
        );
        // Act
        var response = await Client.PutAsJsonAsync($"banks/update/{_bank1.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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