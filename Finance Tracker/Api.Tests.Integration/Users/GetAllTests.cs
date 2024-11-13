using System.Net.Http.Json;
using Api.Dtos.Users;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
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
        var response = await Client.GetAsync("users/getall");

        response.EnsureSuccessStatusCode();
        
        var usersInDb = await Context.Users.ToListAsync();
        
        Assert.Contains(usersInDb, user => user.Login == _mainUser.Login); 
        Assert.Contains(usersInDb, user => user.Login == _adminUser.Login); 
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
