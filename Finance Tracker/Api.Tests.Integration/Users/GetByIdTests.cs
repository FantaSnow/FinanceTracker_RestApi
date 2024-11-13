using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users;

public class GetByIdTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _adminUser;

    public GetByIdTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _adminUser = UsersData.AdminUser();
    }
    
    [Fact]
    public async Task GetById_Success_WhenUserExists()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"users/getbyid/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var userInDb = await Context.Users.FindAsync(userId);
        Assert.NotNull(userInDb);
        Assert.Equal(_mainUser.Login, userInDb?.Login);
    }

    [Fact]
    public async Task GetById_Fails_WhenUserNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"users/getbyid/{nonExistentUserId}");

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
