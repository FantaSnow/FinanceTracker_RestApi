using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos.Categorys;
using Api.Dtos.Transactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Categorys;

public class CreateTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Category _category1;
    private readonly Category _category2;

    private readonly User _mainUser;
    private readonly User _secondUser;
    private readonly User _adminUser;
    
    public CreateTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondUser = UsersData.AnotherUser();
        _adminUser = UsersData.AdminUser();
        
        _category1 = CategorysData.Category1();
        _category2 = CategorysData.Category2();
    }
    [Fact]
    public async Task CreateCategory_Success_WhenAdminCreatesCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_adminUser.Login, _adminUser.Password);
        var request = new CategoryCreateDto
        (
            Name: "New Category"
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync("categorys/create/", request);

        response.EnsureSuccessStatusCode();
        var createdCategory = await Context.Categorys
            .FirstOrDefaultAsync(c => c.Name == "New Category");

        Assert.NotNull(createdCategory);
    }

    [Fact]
    public async Task CreateCategory_Fails_WhenUserTriesToCreateCategory()
    {
        var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
        var request = new CategoryCreateDto
        (
            Name: "Unauthorized Category"
        );

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var response = await Client.PostAsJsonAsync("categorys/create/", request);

        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_Fails_WhenUserNotAuthorized()
    {
        var request = new CategoryCreateDto
        (
            Name: "No Auth Category"
        );

        var response = await Client.PostAsJsonAsync("categorys/create/", request);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    public async Task InitializeAsync()
    {
        await Context.Users.AddRangeAsync(_mainUser, _adminUser, _secondUser);
        await Context.Categorys.AddRangeAsync(_category1, _category2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Categorys.RemoveRange(Context.Categorys);
        await SaveChangesAsync();
    }
}