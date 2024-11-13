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

public class GetByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Bank _bank1;
    private readonly Bank _bank2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public GetByIdTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _bank1 = BanksData.Bank1(_mainUser.Id);
        _bank2 = BanksData.Bank2(_secondUser.Id);
    }
    [Fact]
    public async Task GetById_Success_WhenBankExists()
    {
        // Arrange
        var bankId = _bank1.Id; 

        // Act
        var response = await Client.GetAsync($"banks/getById/{bankId}");

        // Assert
        response.EnsureSuccessStatusCode();

        var bankFromDb = await Context.Banks
            .Where(b => b.Id == bankId)
            .FirstOrDefaultAsync();

        bankFromDb.Should().NotBeNull();
        bankFromDb.Id.Should().Be(bankId);
        bankFromDb.Name.Should().Be(_bank1.Name);  
        bankFromDb.BalanceGoal.Should().Be(_bank1.BalanceGoal);  

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