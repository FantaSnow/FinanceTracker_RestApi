using System.Net;
using System.Net.Http.Headers;
using Domain.Banks;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Statistics;

public class GetByTimeAndCategoryTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Transaction _transaction1;
    private readonly Transaction _transaction2;
    
    private readonly Category _category1;
    private readonly Category _category2;
    
    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public GetByTimeAndCategoryTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
        
        _transaction1 = TranasctionsData.Transactin1(_mainUser.Id, _category1.Id);
        _transaction2 = TranasctionsData.Transactin2(_secondUser.Id, _category2.Id);
    }
    [Fact]
    public async Task GetByTimeAndCategory_Fails_WhenUserNotAuthorized()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddMonths(-1).ToString("MM-dd-yyyy");
        var endDate = DateTime.UtcNow.AddMonths(1).ToString("MM-dd-yyyy");
        var categoryId = _category1.Id.Value;
        var userId = _mainUser.Id.Value;

        // Act
        var response = await Client.GetAsync($"statistics/getByTimeAndCategory/{startDate}/{endDate}/{categoryId}/user=/{userId}");
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    [Fact]
    public async Task GetByTimeAndCategory_Succeeds_ForAdmin_WhenAccessingOthersData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddMonths(-1).ToString("MM-dd-yyyy");
        var endDate = DateTime.UtcNow.AddMonths(1).ToString("MM-dd-yyyy");
        var categoryId = _category1.Id.Value;
        var userId = _secondUser.Id.Value; 
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.GetAsync($"statistics/getByTimeAndCategory/{startDate}/{endDate}/{categoryId}/user=/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByTimeAndCategory_Fails_ForUser_WhenAccessingOthersData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddMonths(-1).ToString("MM-dd-yyyy");
        var endDate = DateTime.UtcNow.AddMonths(1).ToString("MM-dd-yyyy");
        var categoryId = _category1.Id.Value;
        var targetUserId = _secondUser.Id.Value; 
        
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.GetAsync($"statistics/getByTimeAndCategory/{startDate}/{endDate}/{categoryId}/user=/{targetUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetByTimeAndCategory_Succeeds_ForUser_WhenAccessingOwnData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddMonths(-1).ToString("MM-dd-yyyy");
        var endDate = DateTime.UtcNow.AddMonths(1).ToString("MM-dd-yyyy");
        var categoryId = _category1.Id.Value;
        var userId = _mainUser.Id.Value; 
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        // Act
        var response = await Client.GetAsync($"statistics/getByTimeAndCategory/{startDate}/{endDate}/{categoryId}/user=/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Categorys.AddRangeAsync(_category1, _category2);
        await Context.Transactions.AddRangeAsync(_transaction1, _transaction2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Categorys.RemoveRange(Context.Categorys);
        Context.Transactions.RemoveRange(Context.Transactions);
        await SaveChangesAsync();
    }
}