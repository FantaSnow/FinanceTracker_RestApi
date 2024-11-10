using System.Net.Http.Json;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users;

public class GetBalanceByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _adminUser;

    public GetBalanceByIdTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _adminUser = UsersData.AdminUser();
    }
    
    [Fact]
    public async Task GetBalanceById_Success_WhenUserExists()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"users/getbalance/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var userBalance = await response.Content.ReadFromJsonAsync<decimal>();
        Assert.Equal(_mainUser.Balance, userBalance);
    }
    
    [Fact]
    public async Task GetBalanceById_Fails_WhenUserNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid(); 

        // Act
        var response = await Client.GetAsync($"users/getbalance/{nonExistentUserId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }


    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}