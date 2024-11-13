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

public class DeleteTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public DeleteTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
   [Fact]
    public async Task DeleteBank_Succeeds_WhenAdminDeletesBankForAnyUser()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var bankId = _bank1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"banks/delete/{bankId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedBank = await Context.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
        deletedBank.Should().BeNull();
    }

    [Fact]
    public async Task DeleteBank_Succeeds_WhenUserDeletesOwnBank()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var bankId = _bank1.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"banks/delete/{bankId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var deletedBank = await Context.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
        deletedBank.Should().BeNull();
    }

    [Fact]
    public async Task DeleteBank_Fails_WhenUserTriesToDeleteAnotherUsersBank()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var bankToDelete = await Context.Banks.FirstOrDefaultAsync(b => b.Name == _bank2.Name);
        var bankId = bankToDelete?.Id;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.DeleteAsync($"banks/delete/{bankId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteBank_Fails_WhenUserIsNotAuthorized()
    {
        var bankToDelete = await Context.Banks.FirstOrDefaultAsync(b => b.Name == _bank1.Name);
        var bankId = bankToDelete?.Id;

        var response = await Client.DeleteAsync($"banks/delete/{bankId}");

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