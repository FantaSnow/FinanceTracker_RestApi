using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users;

public class GetAllTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _adminUser;

    public GetAllTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _adminUser = UsersData.AdminUser();
    }
    
    [Fact]
    public async Task GetAllUsers_Success()
    {
        // Arrange
        
        // Act
        var response = await Client.GetAsync("users/getall");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        Assert.Contains(users, user => user.Login == _mainUser.Login); 
        Assert.Contains(users, user => user.Login == _adminUser.Login); 
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