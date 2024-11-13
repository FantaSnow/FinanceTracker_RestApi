using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users;

public class AddToBalance : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;

    public AddToBalance(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
    }
    
    [Fact]
    public async Task AddToBalance_Success_WhenUserIsAuthorized()
    {
        // Arrange
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var userId = _mainUser.Id;
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{userId}", updateBalanceRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await Context.Users.FindAsync(userId);
        updatedUser?.Balance.Should().Be(newBalance);
    }
    
    [Fact]
    public async Task AddToBalanceAnotherPeople_Success_WhenUserIsAuthorizedButNotAdmin()
    {
// Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var userId = _secondUser.Id;
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{userId}", updateBalanceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

    }
    
    [Fact]
    public async Task AddToBalanceAnotherPeople_Success_WhenUserIsAuthorizedButAdmin()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var userId = _secondUser.Id;
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{userId}", updateBalanceRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await Context.Users.FindAsync(userId);
        updatedUser?.Balance.Should().Be(newBalance);
    }
    
    [Fact]
    public async Task AddToBalance_Fails_WhenNoAuthTokenProvided()
    {
        // Arrange
        var userId = _mainUser.Id;
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{userId}", updateBalanceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task AddToBalance_Fails_WhenInvalidAuthTokenProvided()
    {
        // Arrange
        var invalidAuthToken = "invalid_token";
        var userId = _mainUser.Id;
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidAuthToken);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{userId}", updateBalanceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task AddToBalance_Fails_WhenUserNotFound()
    {
        // Arrange
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var nonExistentUserId = Guid.NewGuid();
        var newBalance = 100.00m;
        var updateBalanceRequest = new UserUpdateBalanceDto(Balance: newBalance);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.PutAsJsonAsync($"users/AddToBalance/{nonExistentUserId}", updateBalanceRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}
